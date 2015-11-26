using Humanizer;
using HtmlTags;
using Jagi.Helpers;
using Jagi.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

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

        /// <summary>
        /// 建立 Input Html Tag，會判斷 
        /// 1. Date: 使用 Input-group 的方式，因此必須要額外判斷 disabled，如果是，則當作一般的 text input；否則會無法控制
        /// 2. text input: 預設的輸入
        /// 3. select: 使用 selectOptions 組合成 options 直接變成下拉選單的選項
        /// 4. textarea
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="selectOptions"></param>
        /// <param name="disabled"></param>
        /// <returns></returns>
        public virtual HtmlTag GetInput(FormGroupType type, string value,
            Dictionary<string, string> selectOptions = null, bool disabled = false)
        {
            if (_metadata.PropertyName.EndsWith("Date", StringComparison.OrdinalIgnoreCase) && !disabled)
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

            if (input.Attr("type") != "checkbox")
                input.AddClass("form-control");

            input.Attr("name", this.Name)
                 .Attr("ng-model", this.Expression);

            return input;
        }

        public virtual void ApplyValidationToInput(HtmlTag input)
        {
            if (this.Metadata.IsRequired)
            {
                AddRquiredAttribute(input);
            }

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

        protected void AddRquiredAttribute(HtmlTag input)
        {
            if (input.Attr("type") != "checkbox")
                input.Attr(ConstantString.VALIDATION_REQUIRED_FIELD, "");
            else
            {
                // checkbox 當值是 false 時候，會造成 required invalid，因此一定要使用 ng-required
                input.Attr("ng-required", "value.length == 0");
            }
        }

        public virtual void ApplyCustomizedAttributes(HtmlTag input, string attr, RouteValueDictionary attrs = null)
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

        private bool HasDropdownAttribute()
        {
            var propMetadata = _metadata.Properties.Where(e => {
                var attribute = _metadata.ModelType.GetProperty(e.PropertyName)
                    .GetCustomAttributes(typeof(DropdownAttribute), false)
                    .FirstOrDefault() as DropdownAttribute;
                return attribute != null;
            });
            var dropdowns = _metadata.ModelType.GetCustomAttributes(typeof(DropdownAttribute), false);
            if (dropdowns.Any()) { }
            throw new NotImplementedException();
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

            if (inputType == "text"
                && (_metadata.ModelType == typeof(bool) || _metadata.ModelType == typeof(bool?)))
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
                .AddClass("btn")
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

        protected virtual string GetDefaultDisplayName()
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