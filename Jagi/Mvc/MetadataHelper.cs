//using Jagi.Helpers;
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

    public static class MetadataHelper
    {
        private const string _required = "required";
        private const string _requiredMessage = "欄位 {0} 必須要輸入";

        private const string _minLength = "minLength";
        private const string _minLengthMessage = "欄位 {0} 最小必須要輸入 {1} 長度 ";

        private const string _maxLength = "maxLength";
        private const string _maxLengthMessage = "欄位 {0} 最大長度不可超過 {1}";

        public static IHtmlString ValidationsFor<TModel>(this HtmlHelper helper)
        {
            List<PropertyRule> propRules = new List<PropertyRule>();
            string result = string.Empty;
            foreach (var prop in typeof(TModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.GetCustomAttributes().OfType<HiddenInputAttribute>().Any()) continue;

                var propertyLambda = MakeLambda<TModel>(prop);
                var lambdaExpressionGeneric = typeof(ModelMetadata).GetMethod("FromLambdaExpression");
                var lambdaExpressionMethod = lambdaExpressionGeneric.MakeGenericMethod(typeof(TModel), prop.PropertyType );
                var metadata = (ModelMetadata)lambdaExpressionMethod.Invoke(null, new[] { propertyLambda, new ViewDataDictionary<TModel>() });

                if (metadata.IsComplexType || metadata.PropertyName.ToLower().Equals("id")) // 遇到 id 不予以處理，不提供 validation message
                    continue;

                string displayName = metadata.DisplayName ?? metadata.PropertyName.Humanize(LetterCasing.Title);
                PropertyRule propRule = new PropertyRule { PropertyName = metadata.PropertyName, Rules = new Dictionary<string, dynamic>() };

                if (prop.GetCustomAttributes().OfType<StringLengthAttribute>().Any())
                {
                    var attr = prop.GetCustomAttributes().OfType<StringLengthAttribute>().FirstOrDefault();
                    if (attr.MinimumLength > 0)
                        propRule.Rules.Add(_minLength, new {
                            message = attr.ErrorMessage ?? _minLengthMessage.FormatWith(metadata.PropertyName, attr.MinimumLength),
                            parameters = attr.MinimumLength });
                    if (attr.MaximumLength > 0)
                        propRule.Rules.Add(_maxLength, new {
                            message = attr.ErrorMessage ?? _maxLengthMessage.FormatWith(metadata.PropertyName, attr.MaximumLength),
                            parameters = attr.MaximumLength
                        });
                }

                if (metadata.IsRequired)
                {
                    propRule.Rules.Add(_required, new { message = _requiredMessage.FormatWith(displayName) });
                }

                if (propRule.Rules.Count > 0)
                    propRules.Add(propRule);
            }
            
            return helper.Raw(Helpers.JsonHelper.ToJson(propRules));
        }

        //Constructs a lambda of the form x => x.PropName
        public static object MakeLambda<TModel>(PropertyInfo prop)
        {
            var parameter = Expression.Parameter(typeof(TModel), "x");
            var property = Expression.Property(parameter, prop);
            var funcType = typeof(Func<,>).MakeGenericType(typeof(TModel), prop.PropertyType);

            //x => x.PropName
            return Expression.Lambda(funcType, property, parameter);
        }
    }
}
