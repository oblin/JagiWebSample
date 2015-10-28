using HtmlTags;
using Jagi.Database.Models;
using Jagi.Helpers;
using Jagi.Interface;
using Jagi.Mvc;
using Jagi.Mvc.Angular;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Jagi.Database.Mvc
{
    public class ColumnsHtmlTag : AngularHtmlTag
    {
        private Cache.ColumnsCache _columns;
        private TableSchema _column;

        public override ModelMetadata Metadata
        {
            get { return _metadata; }
            set
            {
                _metadata = value;
                var tableName = _columns.GetRelativeTableName(Metadata.ContainerType.Name);
                _column = _columns.Get(tableName, Metadata.PropertyName);
            }
        }

        public override PropertyRule Validations
        {
            get { return _validations; }
            set
            {
                _validations = value;
                if (_column != null)
                {
                    if (_validations == null)
                    {
                        _validations = new PropertyRule 
                            { PropertyName = _column.ColumnName, Rules = new Dictionary<string, dynamic>() };
                    }

                    var displayName = GetLabelFromColumns();
                    // 將 Table Schema 定義的 validation rule 放入
                    AddRequiredValidation(displayName);
                    AddStringLengthValidation(displayName);
                    AddNumberRangeValaidation(displayName);
                }
            }
        }

        public ColumnsHtmlTag()
        {
            _columns = new Cache.ColumnsCache();
        }

        public override HtmlTag GetLabel(FormGroupLayout layout)
        {
            if (_column == null || !string.IsNullOrEmpty(Metadata.DisplayName))
                return base.GetLabel(layout);

            string columnLabel = GetLabelFromColumns();

            return ComposeLabelTag(layout, columnLabel);
        }

        public override HtmlTag GetInput(FormGroupType type, string value, Dictionary<string, string> selectOptions = null)
        {
            if (_column == null || type != FormGroupType.Default)
                return base.GetInput(type, value, selectOptions);

            // 只處理 FormGroupType.Default 情況，避免使用者直接在畫面上指定
            if (_column.DataType == FieldType.Int32 || _column.DataType == FieldType.Decimal)
            {
                return base.GetInput(FormGroupType.Number, null, null);
            }
            if (_column.DataType == FieldType.String && !string.IsNullOrEmpty(_column.DropdwonKey))
            {
                var options = GetCodeCacheOptions(_column.DropdwonKey);
                return base.GetInput(FormGroupType.Dropdown, null, options);
            }
            return base.GetInput(type, value, selectOptions);
        }

        private Dictionary<string, string> GetCodeCacheOptions(string p)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            return result;
        }

        private string GetLabelFromColumns()
        {
            string label = string.Empty;
            if (!string.IsNullOrEmpty(_column.DisplayName))
                label = _column.DisplayName;
            else
            {
                label = GetDefaultDisplayName();
            }

            return label;
        }

        private void AddNumberRangeValaidation(string displayName)
        {
            if (_column.DataType == FieldType.Decimal || _column.DataType == FieldType.Int32)
            {
                if (_column.MinValue.HasValue)
                {
                    _validations.Rules.Add(ConstantString.VALIDATION_MIN_VALUE,
                        new
                        {
                            message = ConstantString.VALIDATION_MIN_MESSAGE
                                .FormatWith(displayName, _column.MinValue),
                            parameters = _column.MinValue
                        });
                }
                if (_column.MaxValue.HasValue)
                {
                    _validations.Rules.Add(ConstantString.VALIDATION_MAX_VALUE,
                        new
                        {
                            message = ConstantString.VALIDATION_MAX_MESSAGE
                                .FormatWith(displayName, _column.MaxValue),
                            parameters = _column.MaxValue
                        });
                }
            }
        }

        private void AddStringLengthValidation(string displayName)
        {
            if (_column.DataType == FieldType.String)
            {
                if (_column.StringMaxLength > 0
                    && !_validations.Rules.ContainsKey(ConstantString.VALIDATION_MAXLENGTH_FIELD))
                {
                    _validations.Rules.Add(ConstantString.VALIDATION_MAXLENGTH_FIELD, new
                    {
                        message = ConstantString.VALIDATION_MAXLENGTH_MESSAGE
                            .FormatWith(displayName, _column.StringMaxLength),
                        parameters = _column.StringMaxLength
                    });
                }
                if (_column.StringMinLength.HasValue
                    && !_validations.Rules.ContainsKey(ConstantString.VALIDATION_MINLENGTH_FIELD))
                {
                    _validations.Rules.Add(ConstantString.VALIDATION_MINLENGTH_FIELD, new
                    {
                        message = ConstantString.VALIDATION_MINLENGTH_MESSAGE
                            .FormatWith(displayName, _column.StringMinLength),
                        parameters = _column.StringMinLength
                    });
                }
            }
        }

        private void AddRequiredValidation(string displayName)
        {
            if (!_column.Nullable)
            {
                if (!_validations.Rules.ContainsKey(ConstantString.VALIDATION_REQUIRED_FIELD))
                    _validations.Rules.Add(ConstantString.VALIDATION_REQUIRED_FIELD,
                        new
                        {
                            message = ConstantString.VALIDATION_REQUIRED_MESSAGE
                                .FormatWith(displayName)
                        });
            }
        }
    }
}
