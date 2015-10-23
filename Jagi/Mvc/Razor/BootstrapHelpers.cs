using Jagi.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Jagi.Mvc.Razor
{
    public static class BootstrapHelpers
    {
        /// <summary>
        /// 設定 Bootstrap 預設的 class 與在 TableSchema 中設定的 Tooltips
        /// </summary>
        public static IHtmlString BootstrapLabelFor<TModel, TProp>(
            this HtmlHelper<TModel> helper, Expression<Func<TModel, TProp>> property,
            Dictionary<string, object> htmlAttributes, int col = 5)
        {
            htmlAttributes = SetDefaultHtmlAttributes<TModel, TProp>(property, htmlAttributes);
            htmlAttributes.Add(@"class", "control-label " + ConstantString.GRID_COLUMN + col);
            return helper.LabelFor(property, htmlAttributes);
        }

        /// <summary>
        /// 設定 Bootstrap 預設的 class 與在 TableSchema 中設定的 Tooltips
        /// </summary>
        public static IHtmlString BootstrapLabelFor<TModel, TProp>(
            this HtmlHelper<TModel> helper, Expression<Func<TModel, TProp>> property, int col = 5)
        {
            return BootstrapLabelFor(helper, property, null, col);
            //return helper.LabelFor(property, new { @class = "control-label " + ConstantString.GRID_COLUMN + col });
        }

        /// <summary>
        /// 設定 Bootstrap 預設的 class
        /// 請注意，在此不能使用 ModelMetadata 取出 Tooltips，必須要配合 EditorForModel 才可以經由 Object.cshtml 提供 Tooltips 設定
        /// </summary>
        public static IHtmlString BootstrapLabel(this HtmlHelper helper, string propertyName, Dictionary<string, object> htmlAttributes, int col = 5)
        {
            if (htmlAttributes == null)
                htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.Add(@"class", "control-label " + ConstantString.GRID_COLUMN + col);

            return helper.Label(propertyName, htmlAttributes);
        }

        /// <summary>
        /// 設定 Bootstrap 預設的 class
        /// </summary>
        public static IHtmlString BootstrapLabel(this HtmlHelper helper, string propertyName, int col = 5)
        {
            return BootstrapLabel(helper, propertyName, null, col);
            //return helper.Label(propertyName, new { @class = "control-label " + colGrid + col });
        }

        private static Dictionary<string, object> SetDefaultHtmlAttributes<TModel, TProp>(
            Expression<Func<TModel, TProp>> property, Dictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                var model = ModelMetadata.FromLambdaExpression(property, new ViewDataDictionary<TModel>());
                bool hasDefaultAttrs = model.AdditionalValues.Keys.Contains(ConstantString.LABEL_HTML_ATTRIBUTES);
                if (hasDefaultAttrs)
                    htmlAttributes = (Dictionary<string, object>)model.AdditionalValues[ConstantString.LABEL_HTML_ATTRIBUTES];
                else
                    htmlAttributes = new Dictionary<string, object>();
            }
            return htmlAttributes;
        }

        #region For Form Tag
        /// <summary>
        /// 限制條件，僅提供給標準的 id 參數使用，如果 post 不使用 id，請用標準的寫法
        /// </summary>
        /// <param name="id">如果不提供，則取出 request route id</param>
        /// <param name="actionName">如果不提供，則取出 request route action</param>
        /// <param name="controllerName">如果不提供，則取出 request route controller</param>
        /// <param name="areaName">如果不提供，則取出 request route area</param>
        /// <param name="htmlAttributes"></param>
        public static MvcForm BootstrapForm(this HtmlHelper helper, object id = null,
            string actionName = null, string controllerName = null,
            string areaName = null, object htmlAttributes = null)
        {
            if (string.IsNullOrEmpty(actionName))
                actionName = helper.ViewContext.Controller.ValueProvider.GetValue("action").RawValue.ToString();
            if (string.IsNullOrEmpty(controllerName))
                controllerName = helper.ViewContext.Controller.ValueProvider.GetValue("controller").RawValue.ToString();
            if (string.IsNullOrEmpty(areaName))
            {
                object areaTemp;
                helper.ViewContext.RouteData.DataTokens.TryGetValue("area", out areaTemp);
                areaName = areaTemp == null ? string.Empty : "/" + areaTemp.ToString();
            }
            if (id == null)
            {
                id = helper.ViewContext.Controller.ValueProvider.GetValue("id").RawValue.ToString();
            }

            string url = areaName + "/" + controllerName + "/" + actionName + "/" + id.ToString();

            object defaultAttributes = new { @class = "form-horizontal" };
            var attributes = new RouteValueDictionary { { "class", "form-horizontal" } };
            RouteValueDictionary htmls = new RouteValueDictionary(htmlAttributes);

            foreach (var item in htmls)
            {
                attributes.Add(item.Key, item.Value);
            }

            return helper.FormHelper(url, FormMethod.Post, attributes);
        }

        private static MvcForm FormHelper(this HtmlHelper htmlHelper, string formAction, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            var builder = new TagBuilder("form");
            builder.MergeAttributes<string, object>(htmlAttributes);
            builder.MergeAttribute("action", formAction);
            builder.MergeAttribute("method", HtmlHelper.GetFormMethodString(method), true);
            htmlHelper.ViewContext.Writer.Write(builder.ToString(TagRenderMode.StartTag));
            var form = new MvcForm(htmlHelper.ViewContext);
            return form;
        }
        #endregion
        #region For ActionLink
        /// <summary>
        /// Save 存檔 button
        /// </summary>
        /// <param name="actionName">action name</param>
        /// <param name="controllerName">controller name</param>
        public static MvcHtmlString BootstrapSaveButton(this HtmlHelper helper,
            string actionName = null, string controllerName = null)
        {
            string htmlAttributes = "btn btn-primary";
            string icon = "glyphicon glyphicon-save";
            string text = "存檔";
            string type = string.Empty;
            if (string.IsNullOrEmpty(actionName))
                type = "Submit";

            return helper.BootstrapButton(icon, text, actionName, controllerName, htmlAttributes, type);
        }

        /// <summary>
        /// Delete 刪除 a href，主要為呼叫不同的 Controller，因此必須要輸入 action & controller Name
        /// </summary>
        /// <param name="actionName">必須要輸入</param>
        /// <param name="controllerName">必須要輸入</param>
        public static MvcHtmlString BootstrapDeleteAction(this HtmlHelper helper,
            string actionName, string controllerName, object routeValues = null)
        {
            //string htmlAttributes = "btn btn-danger";
            object htmlAttributes = new { @class = "btn btn-danger DeleteBtn" };
            string icon = "glyphicon glyphicon-trash";
            string text = "刪除";
            return helper.BootstrapActionLink(icon, text, actionName, controllerName, routeValues, htmlAttributes);
        }

        /// <summary>
        /// Delete 刪除 button （注意預設 action = "Delete"），主要為呼叫原來的 Controller
        /// </summary>
        /// <param name="actionName">action name (預設為 "Delete")，如果不輸入，則 ControllerName 也不可以輸入，否則會出錯</param>
        /// <param name="controllerName"></param>
        public static MvcHtmlString BootstrapDeleteButton(this HtmlHelper helper,
            string actionName = "Delete", string controllerName = null)
        {
            string htmlAttributes = "btn btn-danger DeleteBtn";
            string icon = "glyphicon glyphicon-trash";
            string text = "刪除";
            return helper.BootstrapButton(icon, text, actionName, controllerName, htmlAttributes);
        }

        /// <summary>
        /// Cancel 離開 a href 連結
        /// </summary>
        /// <param name="actionName">action name</param>
        /// <param name="controllerName">controller name</param>
        /// <param name="routeValues">額外的 routing parameters</param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapCancelAction(this HtmlHelper helper,
            string actionName, string controllerName = null, object routeValues = null)
        {
            object htmlAttributes = new { @class = "btn btn-warning" };
            string icon = "glyphicon glyphicon-export";
            string text = "離開";
            return helper.BootstrapActionLink(icon, text, actionName, controllerName, routeValues, htmlAttributes);
        }

        /// <summary>
        /// Create 新增 a href 連結
        /// </summary>
        /// <param name="actionName">action name</param>
        /// <param name="controllerName">controller name</param>
        /// <param name="routeValues">額外的 routing parameters</param>
        public static MvcHtmlString BootstrapCreateAction(this HtmlHelper helper, string actionName, string controllerName, object routeValues)
        {
            object htmlAttributes = new { @class = "btn btn-success" };
            string icon = "glyphicon glyphicon-open";
            string text = "新增";
            return helper.BootstrapActionLink(icon, text, actionName, controllerName, routeValues, htmlAttributes);
        }

        /// <summary>
        /// Edit 編輯 a href 連結
        /// </summary>
        /// <param name="actionName">action name</param>
        /// <param name="controllerName">controller name</param>
        /// <param name="routeValues">額外的 routing parameters</param>
        public static MvcHtmlString BootstrapEditAction(this HtmlHelper helper, string actionName, string controllerName, object routeValues, object htmlAttributes = null)
        {
            var routes = HtmlHelper.AnonymousObjectToHtmlAttributes(routeValues);
            string text = "編輯";
            return BootstrapEditAction(helper, text, actionName, controllerName, routes, htmlAttributes);
        }

        /// <summary>
        /// Edit 編輯 a href 連結
        /// </summary>
        /// <param name="text">顯示名稱</param>
        /// <param name="actionName">action name</param>
        /// <param name="controllerName">controller name</param>
        /// <param name="routeValues">額外的 routing parameters</param>
        public static MvcHtmlString BootstrapEditAction(this HtmlHelper helper, string text, string actionName, string controllerName, RouteValueDictionary routeValues, object htmlAttributes)
        {
            var routeDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            routeDictionary.Add("class", "btn btn-sm btn-success");
            string icon = "glyphicon glyphicon-pencil";
            var builder = new TagBuilder("span");
            builder.MergeAttribute("class", icon);
            var link = helper.ActionLink(text + "[replaceme] ", actionName, controllerName, routeValues, routeDictionary).ToHtmlString();
            return new MvcHtmlString(link.Replace("[replaceme]", builder.ToString()) + "\n");
        }

        /// <summary>
        /// Detail 查看細項 a href 連結
        /// </summary>
        /// <param name="actionName">action name</param>
        /// <param name="controllerName">controller name</param>
        /// <param name="routeValues">額外的 routing parameters</param>
        public static MvcHtmlString BootstrapDetailAction(this HtmlHelper helper, string actionName, string controllerName, RouteValueDictionary routeValues, object htmlAttributes)
        {
            var routeDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            routeDictionary.Add("class", "btn btn-sm btn-primary");
            string icon = "glyphicon glyphicon-indent-left";
            var builder = new TagBuilder("span");
            builder.MergeAttribute("class", icon);
            string text = "查閱";
            var link = helper.ActionLink(text + "[replaceme] ", actionName, controllerName, routeValues, routeDictionary).ToHtmlString();
            return new MvcHtmlString(link.Replace("[replaceme]", builder.ToString()) + "\n");
        }

        /// <summary>
        /// 提供方式：a href 連結
        /// </summary>
        /// <param name="icon">bootstrap icon 名稱</param>
        /// <param name="text">連結字串</param>
        /// <param name="actionName">action name</param>
        /// <param name="controllerName">controller name</param>
        /// <param name="routeValues">額外的 routing parameters</param>
        /// <param name="htmlAttributes">額外的 html attributes</param>
        public static MvcHtmlString BootstrapActionLink(this HtmlHelper helper,
            string icon, string text, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            var builder = new TagBuilder("span");
            builder.MergeAttribute("class", icon);
            var link = helper.ActionLink(text + "[replaceme] ", actionName, controllerName, routeValues, htmlAttributes).ToHtmlString();
            return new MvcHtmlString(link.Replace("[replaceme]", builder.ToString()) + "\n");
        }

        /// <summary>
        /// 提供 button tag
        /// </summary>
        /// <param name="icon">bootstrap icon 名稱</param>
        /// <param name="text">連結字串</param>
        /// <param name="actionName">action name</param>
        /// <param name="controllerName">controller name</param>
        /// <param name="routeValues">額外的 routing parameters</param>
        /// <param name="classTypes">額外的 html attributes</param>
        public static MvcHtmlString BootstrapButton(this HtmlHelper helper,
            string icon, string text, string actionName, string controllerName, string classTypes, string type = null)
        {
            var buttonBuilder = new TagBuilder("button");
            buttonBuilder.MergeAttribute("class", classTypes);
            if (!string.IsNullOrEmpty(actionName))
                buttonBuilder.MergeAttribute("name", actionName);

            if (!string.IsNullOrEmpty(type))
                buttonBuilder.MergeAttribute("type", type);

            var ibuilder = new TagBuilder("span");
            ibuilder.MergeAttribute("class", icon);

            buttonBuilder.InnerHtml = text + ibuilder.ToString();

            return MvcHtmlString.Create(buttonBuilder.ToString(TagRenderMode.Normal));
        }
        #endregion
    }
    /// <summary>
    /// 提供設定 Form Layout 使用，預設：
    /// 1. ColumnWidth = 3：代表共有四個 column
    /// 2. LabelWidth = 5
    /// 3. ColumnWidth =0 : 直接用 12 - LabelWidth
    /// </summary>
    public class LayoutSetting
    {
        public LayoutSetting()
        {
            ColumnWidth = 3;
            LabelWidth = 5;
        }
        /// <summary>
        /// 設定顯示控制向的欄位寬度，最大值為 12，代表一個頁面只有一個欄位
        /// 預設為 3 (代表一個頁面有四個欄位)
        /// </summary>
        public int ColumnWidth { get; set; }
        /// <summary>
        /// 顯示控制項的LABEL寬度，預設為 5 
        /// </summary>
        public int LabelWidth { get; set; }
        /// <summary>
        /// 預設會用 12 - LabelWidth，一般情況下不需要設定，會自動計算
        /// </summary>
        public int ControlWidth { get; set; }
    }

    public class LayoutSettings : ViewPage<LayoutSetting> { }
}
