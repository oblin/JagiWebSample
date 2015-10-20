using Jagi.Helpers;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Jagi.Mvc.Angular
{
    /// <summary>
    /// 作為 AngularHtmlTag 的 Factory，讓 Client 端可以使用 ServiceLocator 指定要實作的項目
    /// 可以使用 Unity 設定 AngularHtmlTag 的 Deriven Class，如果沒有，必須要設定 new AngularHtmlTag()
    /// </summary>
    public class AngularHtmlFactory
    {
        public static AngularHtmlTag GetHtmlTag<TModel, TProp>(
            Expression<Func<TModel, TProp>> property,
            string expressionPrefix)
        {
            AngularHtmlTag ngControl = null;
            if (ServiceLocator.IsLocationProviderSet)
            {
                ngControl = ServiceLocator.Current.GetInstance<AngularHtmlTag>();
            }
            if (ngControl == null)
                // Use Default Settings
                ngControl = new AngularHtmlTag();

            var metadata = ModelMetadata.FromLambdaExpression(property, new ViewDataDictionary<TModel>());

            ngControl.Metadata = metadata;
            ngControl.Name = ExpressionHelper.GetExpressionText(property);
            ngControl.Expression = property.ExpressionForInternal(expressionPrefix);

            return ngControl;
        }

        public static AngularValidations GetValidator<TModel>()
        {
            AngularValidations ngValidations = null;
            if (ServiceLocator.IsLocationProviderSet)
                ngValidations = ServiceLocator.Current.GetInstance<AngularValidations>();
            if (ngValidations == null)
                ngValidations = new AngularValidations();

            return ngValidations;
        }
    }
}
