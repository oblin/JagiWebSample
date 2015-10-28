using Humanizer;
using HtmlTags;
using Jagi.Helpers;
using Jagi.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web.Mvc;

namespace Jagi.Mvc.Angular
{
    /// <summary>
    /// 使用 DataAnnotation 提供 HtmlTag 產生器，可以繼承後，使用 override 改寫任何一項內容
    /// </summary>
    public class AngularHtmlTag
    {
        protected ModelMetadata _metadata;
        public virtual ModelMetadata Metadata { get { return _metadata; } set { _metadata = value; } }
        public string Name { get; internal set; }
        public string Expression { get; internal set; }
        protected PropertyRule _validations;
        public virtual PropertyRule Validations
        {
            get { return _validations; }
            set
            {
                _validations = value;
            }
        }

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

        public virtual HtmlTag GetLabel(FormGroupLayout layout)
        {
            CheckInitializedCorrection();
            var labelText = GetDefaultDisplayName();

            return ComposeLabelTag(layout, labelText);
        }

        public virtual HtmlTag GetInput(FormGroupType type, string value,
            Dictionary<string, string> selectOptions = null)
        {
            if (_metadata.PropertyName.EndsWith("Date", StringComparison.OrdinalIgnoreCase))
            {
                return GetDateInputTag();
            }

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
                    input = CreateInputTag(type, value, placeholder);
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
                input.Attr(ConstantString.VALIDATION_REQUIRED_FIELD, "");

            if (this.Metadata.DataTypeName == "EmailAddress")
                input.Attr("type", "email");

            if (this.Metadata.DataTypeName == "PhoneNumber")
                input.Attr("pattern", @"[\ 0-9()-]+");

            if (this.Validations != null && this.Validations.Rules.Count > 0)
            {
                foreach (var rule in this.Validations.Rules)
                {
                    if (rule.Key == ConstantString.VALIDATION_REQUIRED_FIELD)
                        continue;
                    var ruleDict = TypeHelper.DynamicToDictionary(rule.Value);

                    var value = ruleDict["parameters"];

                    var key = rule.Key.ToLower();
                    if (key != ConstantString.VALIDATION_MIN_VALUE && key != ConstantString.VALIDATION_MAX_VALUE)
                        key = "ng-" + key;
                    input.Attr(key, value);
                }
                string messages = this.Validations.Rules.ToJson();
                input.Attr("message", messages);
            }
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

        protected virtual HtmlTag CreateInputTag(FormGroupType type, string value, string placeholder)
        {
            var input = new HtmlTag("input");
            string numberClass = GetNumberClass(type);
            if (!string.IsNullOrEmpty(numberClass))
                input.AddClass(numberClass);
            else
                if (!string.IsNullOrEmpty(placeholder))
                    input.Attr("placeholder", placeholder);

            SetInputType(input, type, value);

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

        protected virtual void SetInputType(HtmlTag input, FormGroupType type, string value)
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

                case FormGroupType.RadioButton:
                    inputType = "radio";
                    break;

                case FormGroupType.Number:
                    inputType = "number";
                    break;
            }

            if (inputType == "text" && _metadata.ModelType == typeof(bool))
            {
                inputType = "checkbox";
            }
            else if (inputType == "text" && _metadata.ModelType.IsNumericOrNull())
            {
                inputType = "number";
            }

            if (inputType == "radio" || inputType == "checkbox")
            {
                if (string.IsNullOrEmpty(value))
                    input.Attr("value", "true");
                else
                    input.Attr("value", value);
            }

            input.Attr("type", inputType);

            return;
        }

        protected virtual HtmlTag GetDateInputTag()
        {
            var tag = new HtmlTag("div");
            tag.AddClass("input-group");
            var input = new HtmlTag("input");
            input.AddClass("form-control")
                 .Attr("name", this.Name)
                 .Attr("ng-model", this.Expression)
                 .Attr("type", "text")
                 .Attr("datepicker-popup", "yyyy/MM/dd")
                 .Attr("is-open", "dateStatus.opened");

            ApplyValidationToInput(input);

            var span = new HtmlTag("span").AddClass("input-group-btn");
            var button = new HtmlTag("button")
                .Attr("type", "button")
                .AddClasses("btn", "btn-default")
                .Attr("ng-click", "dateStatus.opened = true")
                .Attr("icon", "fa-calendar");
            span.Append(button);
            return tag.Append(input).Append(span);
        }

        protected HtmlTag ComposeLabelTag(FormGroupLayout layout, string labelText)
        {
            var label = new HtmlTag("label")
                .AddClass("control-label")
                .Attr("for", this.Name)
                .Text(labelText);

            if (layout != null)
                label.AddClass(GetLabelLayout(layout));

            return label;
        }

        protected string GetDefaultDisplayName()
        {
            var labelText = _metadata.DisplayName ?? this.Name.Humanize(LetterCasing.Title);
            return labelText;
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

        private string GetLabelLayout(FormGroupLayout layout)
        {
            var gridNumber = layout.LabelGrid;
            return "col-sm-" + gridNumber.ToString();
        }
    }
}