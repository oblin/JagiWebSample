using Jagi.Database.Cache;
using Jagi.Helpers;
using Jagi.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Jagi.Database.Mvc
{
    public static class ColumnsValidations
    {
        public static PropertyRule ColumnValidation<TModel, TProp>(this TModel entity, Expression<Func<TModel, TProp>> property)
        {
            string tableName;
            ColumnsCache columns = GetTableColumns<TModel>(out tableName);

            if (string.IsNullOrEmpty(tableName))
                return null;

            string propName = ((MemberExpression)property.Body).Member.Name;
            var resultSet = ColumnsValidate(entity, propName);
            return resultSet.FirstOrDefault(p => p.PropertyName == propName);
        }

        public static List<PropertyRule> ColumnsValidate<TModel>(TModel entity, string propName = null)
        {
            List<PropertyRule> result = new List<PropertyRule>();

            string tableName;
            ColumnsCache columns = GetTableColumns<TModel>(out tableName);

            if (string.IsNullOrEmpty(tableName))
                return result;

            foreach (var prop in entity.GetType().GetProperties())
            {
                if (!string.IsNullOrEmpty(propName))
                    if (prop.Name != propName)
                        continue;

                var value = prop.GetGetMethod().Invoke(entity, null);

                var column = columns.Get(tableName, prop.Name);

                if (column == null)
                    continue;

                PropertyRule invalid = null;
                if (!column.Nullable && value == null)
                {
                    invalid = new PropertyRule { PropertyName = prop.Name, Rules = new ConcurrentDictionary<string, dynamic>() };
                    invalid.Rules.TryAdd(ConstantString.VALIDATION_REQUIRED_FIELD, ConstantString.VALIDATION_REQUIRED_MESSAGE.FormatWith(prop.Name));
                }

                if (column.DataType == Interface.FieldType.String)
                {
                    string sValue = Convert.ToString(value);
                    if (column.StringMaxLength > 0)
                    {
                        if (sValue.Length > column.StringMaxLength)
                        {
                            if (invalid == null)
                                invalid = new PropertyRule { PropertyName = prop.Name, Rules = new ConcurrentDictionary<string, dynamic>() };
                            invalid.Rules.TryAdd(
                                ConstantString.VALIDATION_MAXLENGTH_FIELD,
                                ConstantString.VALIDATION_MAXLENGTH_MESSAGE.FormatWith(prop.Name, column.StringMaxLength));
                        }
                    }
                    if (column.StringMinLength > 0)
                    {
                        if (sValue.Length < column.StringMinLength)
                        {
                            if (invalid == null)
                                invalid = new PropertyRule { PropertyName = prop.Name, Rules = new ConcurrentDictionary<string, dynamic>() };
                            invalid.Rules.TryAdd(
                                ConstantString.VALIDATION_MINLENGTH_FIELD,
                                ConstantString.VALIDATION_MINLENGTH_MESSAGE.FormatWith(prop.Name, column.StringMinLength));
                        }
                    }
                }

                if (column.DataType == Interface.FieldType.Decimal || column.DataType == Interface.FieldType.Int32)
                {
                    try
                    {
                        decimal dValue = Convert.ToDecimal(value);
                        if (column.MinValue.HasValue)
                        {
                            if (dValue < column.MinValue)
                            {
                                if (invalid == null)
                                    invalid = new PropertyRule { PropertyName = prop.Name, Rules = new ConcurrentDictionary<string, dynamic>() };
                                invalid.Rules.TryAdd(
                                    ConstantString.VALIDATION_MIN_VALUE,
                                    ConstantString.VALIDATION_MIN_MESSAGE.FormatWith(prop.Name, column.MinValue));
                            }
                        }
                        if (column.MaxValue.HasValue)
                        {
                            if (dValue > column.MaxValue)
                            {
                                if (invalid == null)
                                    invalid = new PropertyRule { PropertyName = prop.Name, Rules = new ConcurrentDictionary<string, dynamic>() };
                                invalid.Rules.TryAdd(
                                    ConstantString.VALIDATION_MAX_VALUE,
                                    ConstantString.VALIDATION_MAX_MESSAGE.FormatWith(prop.Name, column.MaxValue));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (invalid == null)
                            invalid = new PropertyRule { PropertyName = prop.Name, Rules = new ConcurrentDictionary<string, dynamic>() };
                        invalid.Rules.TryAdd("ValidationException", ex.Message);
                    }
                }

                if (invalid != null)
                    result.Add(invalid);
            }

            return result;
        }

        private static ColumnsCache GetTableColumns<TModel>(out string tableName)
        {
            ColumnsCache columns = new ColumnsCache();
            string typeName = typeof(TModel).Name;
            tableName = columns.GetRelativeTableName(typeName);

            return columns;
        }
    }
}
