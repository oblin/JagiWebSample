using HtmlTags;
using Humanizer;
using Jagi.Helpers;
using Jagi.Mvc.Helpers;
using Jagi.Utility;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Jagi.Mvc.Angular
{
    public class AngularModelHelper<TModel>
    {
        protected readonly HtmlHelper _helper;
        private readonly string _expressionPrefix;
        private readonly FormGroupLayout _layout;

       [Dependency("InjectedAngularHtmlMethod")]
        public AngularHtmlTag _control { get; set; }

        public AngularModelHelper(HtmlHelper helper, string expressionPrefix, FormGroupLayout layout = null)
        {
            _helper = helper;
            _expressionPrefix = expressionPrefix;
            _layout = layout;
        }

        /// <summary>
        /// Converts an lambda expression into a camel-cased string, prefixed
        /// with the helper's configured prefix expression, ie:
        /// vm.model.parentProperty.childProperty
        /// </summary>
        public IHtmlString ExpressionFor<TProp>(Expression<Func<TModel, TProp>> property)
        {
            var expressionText = property.ExpressionForInternal(_expressionPrefix);
            return new MvcHtmlString(expressionText);
        }

        /// <summary>
        /// Converts a lambda expression into a camel-cased AngularJS binding expression, ie:
        /// {{vm.model.parentProperty.childProperty}}
        /// </summary>
        public IHtmlString BindingFor<TProp>(Expression<Func<TModel, TProp>> property)
        {
            return MvcHtmlString.Create("{{" + property.ExpressionForInternal(_expressionPrefix) + "}}");
        }

        /// <summary>
        /// Creates a div with an ng-repeat directive to enumerate the specified property,
        /// and returns a new helper you can use for strongly-typed bindings on the items
        /// in the enumerable property.
        /// 使用方式：
        ///     @using(var object = model.Repeat(x => x.objects, "object")) {
        ///         <li>@object.BindingFor(x => x.Title)</li>
        ///         <li>@object.BindingFor(x => x.Name)</li>
        ///     }
        /// </summary>
        public AngularNgRepeatHelper<TSubModel> Repeat<TSubModel>(
            Expression<Func<TModel, IEnumerable<TSubModel>>> property, string variableName)
        {
            var propertyExpression = property.ExpressionForInternal();
            return new AngularNgRepeatHelper<TSubModel>(
                _helper, variableName, propertyExpression);
        }

        private HtmlTag AngularLabelFor(AngularHtmlTag ngControl, FormGroupLayout layout)
        {
            if (layout == null)
                layout = _layout;

            var result = ngControl.GetLabel(layout);

            return result;
        }

        private string GetPropertyStringExpression<TProp>(Expression<Func<TModel, TProp>> property)
        {
            string result = ((LambdaExpression)property).Body.ToString();
            var paramName = property.Parameters[0].Name;
            var paramTypeName = property.Parameters[0].Type.Name;

            return result.Replace(paramName + ".", paramTypeName + ".");
        }

        private HtmlTag AngularEditorFor(AngularHtmlTag ngControl, FormGroupType type, string attr, RouteValueDictionary attrs, Dictionary<string, string> options, string value)
        {
            bool disable = GetDisabledAttributes(attrs);
            HtmlTag input = ngControl.GetInput(type, value, options, disable);

            if (input.IsInputElement())
            {
                ngControl.ApplyValidationToInput(input);
                ngControl.ApplyCustomizedAttributes(input, attr, attrs);
            }

            return input;
        }

        private bool GetDisabledAttributes(RouteValueDictionary attrs)
        {
            if (attrs == null)
                return false;
            return attrs.ContainsKey("disabled");
        }

        public HtmlTag FormGroupFor<TProp>(Expression<Func<TModel, TProp>> property,
            int formGroupGrid, object attrs = null )
        {
            RouteValueDictionary dict = DictionaryHelper.ObjectToRouteValueDictionary(attrs);

            FormGroupLayout layout = new FormGroupLayout((int)formGroupGrid);
            return FormGroupFor(property, attrs: dict, layout: layout);
            //try
            //{
            //    FormGroupLayout layout = new FormGroupLayout((int)formGroupGrid);
            //    return FormGroupFor(property, attrs: dict, layout: layout);
            //}
            //catch (ArgumentOutOfRangeException)
            //{
            //    return FormGroupFor(property, attrs: dict, formGroupGrid: formGroupGrid);
            //}
        }
        
        public HtmlTag FormGroupFor<TProp>(Expression<Func<TModel, TProp>> property,
            object attrs, int? formGroupGrid = null)
        {
            RouteValueDictionary dict = DictionaryHelper.ObjectToRouteValueDictionary(attrs);

            if (formGroupGrid.HasValue)
            {
                return FormGroupFor(property, (int)formGroupGrid, attrs);
            }
            else
            {
                return FormGroupFor(property, attrs: dict);
            }
        }

        /// <summary>
        /// 建立 bootstrap Fromgroup Div 與其所屬的內容，範例：
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property"></param>
        /// <param name="type">可以手動指定 text, checkbox... </param>
        /// <param name="attr">任意指定的 attr，如： ng-click="checkItOut()" </param>
        /// <param name="attrs">同 attr，但是可以有多個，使用 Dict<attrKey, attrValue>，如： { "ng-click", "checkItOut()" }</param>
        /// <param name="options">提供 dropdown options</param>
        /// <param name="value">提供指定的 value，主要是給 checkbox or radio button 使用</param>
        /// <param name="values">提供多個 value，並且會依據每一個 value 產生 input</param>
        /// <param name="formGroupGrid">提供主要 grid layout 數字，僅允許： 3,4,6,8,12 這幾個數字</param>
        /// <param name="layout">輸入new FormGroupGrid(labelGrid, inputGrid, formGrid)</param>
        /// <param name="span">指定 Input 後面的 span (變成 input-addon)，使用 dictionary 輸入 span 的各項屬性</param>
        /// <returns></returns>
        public HtmlTag FormGroupFor<TProp>(Expression<Func<TModel, TProp>> property,
            FormGroupType type = FormGroupType.Default,
            string attr = null,
            RouteValueDictionary attrs = null,
            Dictionary<string, string> options = null,
            string value = null,
            string[] values = null,
            int? formGroupGrid = null,
            FormGroupLayout layout = null,
            Dictionary<string, string> span = null)
        {
            layout = layout ?? _layout;
            formGroupGrid = formGroupGrid ?? (layout != null ? layout.FormGrid : null);

            AngularHtmlTag ngControl = AngularHtmlFactory.GetHtmlTag(property, _expressionPrefix);

            string name = ngControl.GetName();

            HtmlTag formGroup = new HtmlTag("div")
                .AddClasses("form-group", "has-feedback")
                .Attr("form-group-validation", name);

            if (formGroupGrid != null)
                formGroup.AddClass("col-sm-" + formGroupGrid.ToString());

            HtmlTag label = null;
            HtmlTag input = AngularEditorFor(ngControl, type, attr, attrs, options, value);

            if (InputHasRquiredAttr(input))
                formGroup.AddClass(ConstantString.VALIDATION_REQUIRED_FIELD);

            label = AngularLabelFor(ngControl, layout);
            if (layout != null)
            {
                var spanTag = CreateSpan(span);
                input = AppendLayoutInputDiv(layout, input, spanTag);
            }
            return formGroup.Append(label).Append(input);
        }

        private HtmlTag CreateSpan(Dictionary<string, string> span)
        {
            if (span == null)
                return null;
            HtmlTag result = new HtmlTag("span");
            foreach (var item in span)
            {
                if (item.Key == "text" || item.Key == "Text")
                    result.Text(item.Value);
                else
                    result.Attr(item.Key, item.Value);
            }
            if (!result.HasAttr("input-group-addon"))
                result.AddClasses("input-group-addon");
            return result;
        }

        private static bool InputHasRquiredAttr(HtmlTag input)
        {
            if (input.HasAttr(ConstantString.VALIDATION_REQUIRED_FIELD))
                return true;
            foreach (var child in input.Children)
                return InputHasRquiredAttr(child);
            return false;
        }

        private static HtmlTag AppendLayoutInputDiv(FormGroupLayout layout, HtmlTag input, HtmlTag span)
        {
            var layoutDiv = new HtmlTag("div");
            if (span == null)
            {
                input = layoutDiv.Append(input);
            }
            else
            {
                layoutDiv.AddClass("input-group");
                input = layoutDiv.Append(input).Append(span);
            }
            layoutDiv.AddClass("col-sm-" + layout.InputGrid);
            return input;
        }

        private static HtmlTag AppendCheckboxOrRadio(HtmlTag formGroup, HtmlTag input, string type)
        {
            HtmlTag div = new HtmlTag("div").AddClass(type);
            var label = new HtmlTag("label");
            label.Append(input);
            div.Append(label);
            formGroup.Append(div);
            return formGroup;
        }
    }
}