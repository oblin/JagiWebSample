﻿//using Jagi.Helpers;
using Humanizer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Jagi.Mvc
{
    public class PropertyRule
    {
        public string PropertyName { get; set; }
        public Dictionary<string, dynamic> Rules { get; set; }
    }

    public class AngularValidations
    {
        protected const string _requiredField = "required";
        protected const string _requiredMessage = "欄位 【{0}】 必須要輸入";

        protected const string _minLengthField = "minLength";
        protected const string _minLengthMessage = "欄位 【{0}】 最小必須要輸入 {1} 長度 ";

        protected const string _maxLengthField = "maxLength";
        protected const string _maxLengthMessage = "欄位 【{0}】 最大長度不可超過 {1}";

        protected string _displayName;
        protected ModelMetadata _metadata;

        /// <summary>
        /// 回傳 TModel 的所有 Validation 規則，使用 IHtmlString 方式回傳，內容範例：
        /// [{"propertyName":"number","rules":{"required":{"message":"欄位 【數字】 必須要輸入"}}},{"propertyName":"text","rules":{"maxLength":{"message":"欄位 【文本】 最大長度不可超過 5","parameters":5}}}]
        /// 目前僅提供以下規則：
        /// 1. Required
        /// 2. String Length
        /// 必須要搭配 common.js 中的 validater() rule funciton 才能提供前端驗證
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <param name="isCamelCase">是否第一個字母要大寫，預設：是</param>
        /// <returns></returns>
        public IHtmlString ValidationsFor<TModel>(HtmlHelper helper, bool isCamelCase = true)
        {
            List<PropertyRule> propRules = new List<PropertyRule>();
            string result = string.Empty;
            foreach (var prop in typeof(TModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.GetCustomAttributes().OfType<HiddenInputAttribute>().Any()) continue;

                _metadata = GetMetadata<TModel>(prop);

                string propName = _metadata.PropertyName;
                if (isCamelCase)
                    propName = Helpers.JsonHelper.ConvertToCamelCase(propName);

                if (_metadata.IsComplexType || propName.ToLower().Equals("id")) // 遇到 id 不予以處理，不提供 validation message
                    continue;

                _displayName = GetDisplayName(_metadata);

                PropertyRule propRule = new PropertyRule { PropertyName = propName, Rules = new Dictionary<string, dynamic>() };

                AddStringLengthRule(prop, propRule);

                AddRequiredRule(propRule);

                if (propRule.Rules.Count > 0)
                    propRules.Add(propRule);
            }

            return helper.Raw(Helpers.JsonHelper.ToJson(propRules));
        }

        /// <summary>
        /// 設定是否有 Required 的 validation rule
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="propRule"></param>
        /// <returns></returns>
        protected virtual bool AddRequiredRule(PropertyRule propRule)
        {
            bool hadRule = false;
            if (_metadata.IsRequired)
            {
                propRule.Rules.Add(_requiredField, new { message = _requiredMessage.FormatWith(_displayName) });
                hadRule = true;
            }

            return hadRule;
        }

        protected virtual bool AddStringLengthRule(PropertyInfo prop, PropertyRule propRule)
        {
            bool hadRule = false;
            if (prop.GetCustomAttributes().OfType<StringLengthAttribute>().Any())
            {
                var attr = prop.GetCustomAttributes().OfType<StringLengthAttribute>().FirstOrDefault();
                if (attr.MinimumLength > 0)
                {
                    propRule.Rules.Add(_minLengthField, new
                    {
                        message = attr.ErrorMessage ?? _minLengthMessage.FormatWith(_displayName, attr.MinimumLength),
                        parameters = attr.MinimumLength
                    });
                    hadRule = true;
                }
                if (attr.MaximumLength > 0)
                {
                    propRule.Rules.Add(_maxLengthField, new
                    {
                        message = attr.ErrorMessage ?? _maxLengthMessage.FormatWith(_displayName, attr.MaximumLength),
                        parameters = attr.MaximumLength
                    });
                    hadRule = true;
                }
            }

            return hadRule;
        }

        //Constructs a lambda of the form x => x.PropName
        public object MakeLambda<TModel>(PropertyInfo prop)
        {
            var parameter = Expression.Parameter(typeof(TModel), "x");
            var property = Expression.Property(parameter, prop);
            var funcType = typeof(Func<,>).MakeGenericType(typeof(TModel), prop.PropertyType);

            //x => x.PropName
            return Expression.Lambda(funcType, property, parameter);
        }

        protected virtual string GetDisplayName(ModelMetadata metadata)
        {
            return metadata.DisplayName ?? metadata.PropertyName.Humanize(LetterCasing.Title);
        }

        private ModelMetadata GetMetadata<TModel>(PropertyInfo prop)
        {
            var propertyLambda = MakeLambda<TModel>(prop);
            var lambdaExpressionGeneric = typeof(ModelMetadata).GetMethod("FromLambdaExpression");
            var lambdaExpressionMethod = lambdaExpressionGeneric.MakeGenericMethod(typeof(TModel), prop.PropertyType);
            var metadata = (ModelMetadata)lambdaExpressionMethod.Invoke(null, new[] { propertyLambda, new ViewDataDictionary<TModel>() });
            return metadata;
        }
    }
}
