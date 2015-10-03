using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jagi.Helpers;
using System.Web.Mvc;
using HtmlTags;
using Humanizer;

namespace Jagi.Mvc.Angular
{
    public enum FormGroupType
    {
        Default,
        Editor,
        Dropdown,
        Checkbox,
        Number,
        RadioButton,
        Textarea,
        Password,
        Hidden
    }

    enum InputTag
    {
        Input,
        Select,
        Textarea
    }

    public class MetaAngularMapping
    {
        private readonly ModelMetadata _metadata;

        public MetaAngularMapping(ModelMetadata metadata)
        {
            this._metadata = metadata;
        }

        public HtmlTag CreateInput(FormGroupType type, string name, 
            Dictionary<string, string> options = null)
        {
            var inputTag= GetInputTag(type);

            HtmlTag input = null;

            switch (inputTag)
            {
                case InputTag.Textarea:
                    input = CreateTextareaTag(type);
                    break;
                case InputTag.Select:
                    input = CreateSelectTag(type, options);
                    break;
                default:
                    //var labelText = _metadata.DisplayName ?? name.Humanize(LetterCasing.Title);
                    //var placeholder = _metadata.Watermark ?? (labelText + "...");
                    string placeholder = _metadata.Watermark;
                    input = CreateInputTag(type, placeholder);
                    break;
            }

            return input;
        }

        private HtmlTag CreateSelectTag(FormGroupType type, Dictionary<string, string> options)
        {
            var input = new HtmlTag("select");
            if(options != null)
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

        private HtmlTag CreateInputTag(FormGroupType type, string placeholder)
        {
            var input = new HtmlTag("input");
            string numberClass = GetNumberClass(type);
            if (!string.IsNullOrEmpty(numberClass))
                input.AddClass(numberClass);
            else
                if (!string.IsNullOrEmpty(placeholder))
                    input.Attr("placeholder", placeholder);

            string inputType = GetInputType(type);
            input.Attr("type", inputType);

            return input;
        }

        private HtmlTag CreateTextareaTag(FormGroupType type)
        {
            var input = new HtmlTag("textarea");

            return input;
        }

        private InputTag GetInputTag(FormGroupType type)
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

        private string GetInputType(FormGroupType type)
        {
            switch (type)
            {
                case FormGroupType.Password:
                    return "password";
                case FormGroupType.Hidden:
                    return "hidden";
            }

            return "text";
        }

        private string GetNumberClass(FormGroupType type)
        {
            if (_metadata.ModelType.IsNumericOrNull() || type == FormGroupType.Number)
            {
                return "text-right";
            }

            return string.Empty;
        }
    }
}
