using Jagi.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Jagi.Mvc.Helpers
{
    public static class JsonHelper
    {
        /// <summary>
        /// 傳回 IHtmlString 代表該物件的 JSON 型態；請注意，使用時候要用 single quote：
        ///     ng-init='vm.init(@Html.JsonFor(Model))
        /// 理由是因為 C# 字串採用 double quote
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>傳回 IHtmlString 代表該物件的 JSON 型態</returns>
        public static IHtmlString JsonFor<T>(this HtmlHelper helper, T obj)
        {
            // 傳回 IHTMLString 不會做任何的處理
            return helper.Raw(obj.ToJson());
        }

        /// <summary>
        /// 將物件轉換成 Json 表達式； ex: {"number":11,"text":"Sample Text","isChinese":false,"startDate":"0001-01-01T00:00:00","endDate":null,"nullableInt":null}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">任意物件</param>
        /// <param name="includeNull">是否將 null 轉入預設 true，如果不要將 null 轉入，則會沒有 null 值的屬性</param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj, bool isCamelCase = true, bool includeNull = true)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = new JsonConverter[] { new StringEnumConverter() },
                NullValueHandling = includeNull ? NullValueHandling.Include : NullValueHandling.Ignore
            };

            if (isCamelCase)
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// Turns x => x.SomeProperty.SomeValue into "someProperty.someValue"
        /// 請注意前面的 x. 會被刪除
        /// 這裡主要的用途在於提供 angular ngModel 與 razor Model 的命名對應
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string ToCamelCaseName<TModel, TProp>(
            this Expression<Func<TModel, TProp>> property)
        {
            //Turns x => x.SomeProperty.SomeValue into "SomeProperty.SomeValue" 
            var pascalCaseName = ExpressionHelper.GetExpressionText(property);

            //Turns "SomeProperty.SomeValue" into "someProperty.someValue"
            var camelCaseName = ConvertFullNameToCamelCase(pascalCaseName);
            return camelCaseName;
        }

        public static string ExpressionForInternal<TModel, TProp>(this Expression<Func<TModel, TProp>> property,
            string expressionPrefix = null)
        {
            var camelCaseName = property.ToCamelCaseName();

            var expression = !string.IsNullOrEmpty(expressionPrefix)
                ? expressionPrefix + "." + camelCaseName
                : camelCaseName;

            return expression;
        }

        //Converts expressions of the form Some.PropertyName to some.propertyName
        private static string ConvertFullNameToCamelCase(string pascalCaseName)
        {
            var parts = pascalCaseName.Split('.')
                .Select(ConvertToCamelCase);

            return string.Join(".", parts);
        }

        //Borrowed from JSON.NET. Turns a single name into camel case.
        public static string ConvertToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            if (!char.IsUpper(s[0]))
                return s;
            char[] chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                    break;
                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
            }
            return new string(chars);
        }

        /// <summary>
        /// 使用在 AutoMapper 中，將一個欄位用 "-" 分隔到多個 dest 欄位中；例如： <para />
        ///     var optArray = source.EvDesc.Split('-');
        ///     dest.SetJsonBooleanProperty("EvDesc", optArray);
        /// 會放入到 dest.OtherSick1, dest.OtherSick2,...
        /// </summary>
        /// <param name="dest">class 物件</param>
        /// <param name="options">json string array</param>
        public static void SetJsonBooleanProperty(this object dest, string name, string[] options)
        {
            foreach (var opt in options)
            {
                if (string.IsNullOrEmpty(opt))
                    continue;
                var propName = name + opt;
                PropertyInfo prop = dest.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite && prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(dest, true, null);
                }
                else
                {
                    throw new ArgumentException("無法轉換資料, Name: {0}, Value: {1}"
                        .FormatWith(name, string.Join(",", options)));
                }
            }
        }

        /// <summary>
        /// 使用在 AutoMapper 中，將一個欄位用 "-" 分隔到多個 dest 欄位中；例如： <para />
        ///     dest.SetJsonBooleanProperty(() => source.OtherSick, source.OtherSick);
        /// 會放入到 dest.OtherSick1, dest.OtherSick2,...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dest"></param>
        /// <param name="action"></param>
        /// <param name="value"></param>
        public static void SetJsonBooleanProperty<T>(this object dest, Expression<Func<T>> action, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;
            var optArray = value.Split('-');
            var name = LinqHelper.GetExpressionPropertyName(action);
            SetJsonBooleanProperty(dest, name, optArray);
        }
    }
}