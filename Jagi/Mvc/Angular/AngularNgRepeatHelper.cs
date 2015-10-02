using HtmlTags;
using System;
using System.Web.Mvc;

namespace Jagi.Mvc.Angular
{
    /// <summary>
    /// 主要產生 &lt;div ng-repeat='sample in samples'&gt;&lt;/div&gt; 的文字
    /// 可以搭配 .BindingFor() 使用
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class AngularNgRepeatHelper<TModel> : AngularModelHelper<TModel>, IDisposable
    {
        public AngularNgRepeatHelper(HtmlHelper helper,
            string variableName, string propertyExpression)
            : base(helper, variableName)
        {
            var div = new HtmlTag("div");
            div.Attr("ng-repeat",
                string.Format("{0} in {1}", variableName, propertyExpression));
            div.NoClosingTag();

            _helper.ViewContext.Writer.Write(div.ToString());
        }

        void IDisposable.Dispose()
        {
            _helper.ViewContext.Writer.Write("</div>");
        }
    }
}