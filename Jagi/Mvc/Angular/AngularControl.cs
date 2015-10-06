using HtmlTags;
using Humanizer;
using Jagi.Helpers;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Jagi.Mvc.Angular
{
    /// <summary>
    /// 提供基本的 HtmlTag 產生器，可以繼承後，使用 override 改寫任何一項內容
    /// </summary>
    public class AngularHtmlTag
    {
        private ModelMetadata _metadata;
        public ModelMetadata Metadata { get { return _metadata; } internal set { _metadata = value; } }
        public string Name { get; internal set; }
        public string Expression { get; internal set; }

        public AngularHtmlTag()
        {
        }

        public AngularHtmlTag(ModelMetadata metadata)
        {
            _metadata = metadata;
        }

        public virtual string GetName()
        {
            return this.Name;
        }

        public virtual HtmlTag GetLabel()
        {
            CheckInitializedCorrection();
            var labelText = _metadata.DisplayName ?? this.Name.Humanize(LetterCasing.Title);

            var label = new HtmlTag("label")
                .AddClass("control-label")
                .Attr("for", this.Name)
                .Text(labelText);

            return label;
        }

        public virtual HtmlTag GetInputWithValidation(
            FormGroupType type = FormGroupType.Default,
            string attr = null,
            Dictionary<string, string> attrs = null,
            Dictionary<string, string> options = null)
        {
            HtmlTag input = GetInput(type, options);

            ApplyValidationToInput(input);

            ApplyCustomizedAttributes(input, attr, attrs);

            return input;
        }

        public virtual HtmlTag GetInput(FormGroupType type,
            Dictionary<string, string> selectOptions = null)
        {
            var inputTag = GetInputTag(type);

            HtmlTag input = null;

            switch (inputTag)
            {
                case InputTag.Textarea:
                    input = CreateTextareaTag(type);
                    break;

                case InputTag.Select:
                    input = CreateSelectTag(selectOptions);
                    break;

                default:
                    //var labelText = _metadata.DisplayName ?? name.Humanize(LetterCasing.Title);
                    //var placeholder = _metadata.Watermark ?? (labelText + "...");
                    string placeholder = _metadata.Watermark;
                    input = CreateInputTag(type, placeholder);
                    break;
            }

            input.AddClass("form-control")
                 .Attr("name", this.Name)
                 .Attr("ng-model", this.Expression);

            return input;
        }

        public virtual void ApplyValidationToInput(HtmlTag input)
        {
            if (this.Metadata.IsRequired)
                input.Attr("required", "");

            if (this.Metadata.DataTypeName == "EmailAddress")
                input.Attr("type", "email");

            if (this.Metadata.DataTypeName == "PhoneNumber")
                input.Attr("pattern", @"[\ 0-9()-]+");
        }

        public virtual void ApplyCustomizedAttributes(HtmlTag input, string attr, Dictionary<string, string> attrs = null)
        {
            if (attrs != null)
                foreach (var item in attrs)
                    input.Attr(item.Key, item.Value);

            if (!string.IsNullOrEmpty(attr))
            {
                if (attr.Contains("="))
                {
                    var splitAttr = attr.Split('=');
                    string cleanQuote = splitAttr[1].Replace("'", "").Replace("\"", "");
                    input.Attr(splitAttr[0], cleanQuote);
                }
            }
        }

        public virtual HtmlTag CreateSelectTag(Dictionary<string, string> options)
        {
            var input = new HtmlTag("select");
            if (options != null)
            {
                foreach (var item in options)
                {
                    var option = new HtmlTag("option");
                    option.Attr("value", item.Key);
                    option.Text(item.Value);
                    input.Append(option);
                }
            }

            return input;
        }

        protected virtual HtmlTag CreateInputTag(FormGroupType type, string placeholder)
        {
            var input = new HtmlTag("input");
            string numberClass = GetNumberClass(type);
            if (!string.IsNullOrEmpty(numberClass))
                input.AddClass(numberClass);
            else
                if (!string.IsNullOrEmpty(placeholder))
                input.Attr("placeholder", placeholder);

            SetInputType(input, type);

            return input;
        }

        protected virtual HtmlTag CreateTextareaTag(FormGroupType type)
        {
            var input = new HtmlTag("textarea");

            return input;
        }

        protected virtual InputTag GetInputTag(FormGroupType type)
        {
            string tagName = string.Empty;

            switch (type)
            {
                case FormGroupType.Default:
                    return _metadata.DataTypeName == "MultilineText"
                        ? InputTag.Textarea
                        : InputTag.Input;

                case FormGroupType.Textarea:
                    return InputTag.Textarea;

                case FormGroupType.Dropdown:
                    return InputTag.Select;
            }
            return InputTag.Input;
        }

        protected virtual void SetInputType(HtmlTag input, FormGroupType type)
        {
            string inputType = "text";
            switch (type)
            {
                case FormGroupType.Password:
                    inputType = "password";
                    break;

                case FormGroupType.Hidden:
                    inputType = "hidden";
                    break;

                case FormGroupType.Checkbox:
                    inputType = "checkbox";
                    break;
            }

            if (_metadata.ModelType == typeof(bool))
            {
                inputType = "checkbox";
                input.Attr("value", "true");
            }

            input.Attr("type", inputType);

            return;
        }

        private string GetNumberClass(FormGroupType type)
        {
            if (_metadata.ModelType.IsNumericOrNull() || type == FormGroupType.Number)
            {
                return "text-right";
            }

            return string.Empty;
        }

        private void CheckInitializedCorrection()
        {
            if (string.IsNullOrEmpty(this.Name))
                throw new NotImplementedException("this.Name can not be null or empty string");
        }
    }

    /// <summary>
    /// 作為 AngularHtmlTag 的 Factory，讓 Client 端可以使用 ServiceLocator 指定要實作的項目
    /// </summary>
    public class AngularHtmlTagFactory
    {
        public static AngularHtmlTag Get<TModel, TProp>(
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
    }
}