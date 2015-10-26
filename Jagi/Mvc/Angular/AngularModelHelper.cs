using HtmlTags;
using Humanizer;
using Jagi.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Jagi.Mvc.Angular
{
    public class AngularModelHelper<TModel>
    {
        protected readonly HtmlHelper _helper;
        private readonly string _expressionPrefix;
        private readonly FormGroupLayout _layout;

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

        public HtmlTag AngularLabelFor<TProp>(Expression<Func<TModel, TProp>> property, FormGroupLayout layout = null)
        {
            AngularHtmlTag ngControl = AngularHtmlFactory.GetHtmlTag(property, _expressionPrefix);
            if (layout == null)
                layout = _layout;
            return ngControl.GetLabel(layout);
        }

        public HtmlTag AngularEditorFor<TProp>(Expression<Func<TModel, TProp>> property,
            FormGroupType type = FormGroupType.Default,
            string attr = null,
            Dictionary<string, string> attrs = null,
            Dictionary<string, string> options = null,
            string value = null)
        {
            AngularHtmlTag ngControl = AngularHtmlFactory.GetHtmlTag(property, _expressionPrefix);
            HtmlTag input = ngControl.GetInput(type, value, options);

            if (input.IsInputElement())
            {
                ngControl.ApplyValidationToInput(input);

                ngControl.ApplyCustomizedAttributes(input, attr, attrs);
            }

            return input;
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
        /// <returns></returns>
        public HtmlTag FormGroupFor<TProp>(Expression<Func<TModel, TProp>> property, 
            FormGroupType type = FormGroupType.Default,
            string attr = null,
            Dictionary<string, string> attrs = null,
            Dictionary<string, string> options = null,
            string value = null,
            string[] values = null,
            int? formGroupGrid = null,
            FormGroupLayout layout = null)
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
            HtmlTag input = AngularEditorFor(property, type, attr, attrs, options, value);

            if (input.HasAttr("required"))
                formGroup.AddClass("required");

            string checkOrRadioType = string.Empty;
            if (type == FormGroupType.Checkbox || input.Attr("type") == "checkbox")
                checkOrRadioType = "checkbox";
            else if (type == FormGroupType.RadioButton)
                checkOrRadioType = "radio";

            if (string.IsNullOrEmpty(checkOrRadioType))
            {
                // Default for type="text" and textarea
                label = AngularLabelFor(property);
                if (layout != null )
                {
                    input = AppendLayoutInputDiv(layout, input);
                }
                return formGroup.Append(label).Append(input);
            }
            else
            {
                // for type="checkbox" or "radio"
                if (values == null || values.Length == 0)
                {
                    formGroup = AppendCheckboxOrRadio(formGroup, input, checkOrRadioType);
                }
                else
                {
                    foreach (var item in values)
                    {
                        input = AngularEditorFor(property, type, attr, attrs, options, item);
                        formGroup = AppendCheckboxOrRadio(formGroup, input, checkOrRadioType);
                    }
                }
                return formGroup;
            }
        }

        private static HtmlTag AppendLayoutInputDiv(FormGroupLayout layout, HtmlTag input)
        {
            var layoutDiv = new HtmlTag("div");
            layoutDiv.AddClass("col-sm-" + layout.InputGrid);
            input = layoutDiv.Append(input);
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