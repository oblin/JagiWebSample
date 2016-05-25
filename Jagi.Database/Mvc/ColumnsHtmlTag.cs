using HtmlTags;
using Jagi.Database.Models;
using Jagi.Helpers;
using Jagi.Interface;
using Jagi.Mvc;
using Jagi.Mvc.Angular;
using Jagi.Mvc.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;

namespace Jagi.Database.Mvc
{
    public class ColumnsHtmlTag : AngularHtmlTag
    {
        private Cache.ColumnsCache _columns;
        private Cache.CodeCache _codes;

        private TableSchema _column;

        public override ModelMetadata Metadata
        {
            get { return _metadata; }
            set
            {
                _metadata = value;
                var tableName = _columns.GetRelativeTableName(Metadata.ContainerType.Name);
                _column = _columns.Get(tableName, Metadata.PropertyName);
            }
        }

        public override PropertyRule Validations
        {
            get { return _validations; }
            set
            {
                _validations = value;
                if (_column != null)
                {
                    if (_validations == null)
                    {
                        _validations = new PropertyRule
                        { PropertyName = _column.ColumnName, Rules = new Dictionary<string, dynamic>() };
                    }

                    var displayName = GetDefaultDisplayName();
                    // 將 Table Schema 定義的 validation rule 放入
                    AddRequiredValidation(displayName);
                    AddStringLengthValidation(displayName);
                    AddNumberRangeValaidation(displayName);
                    AddPatternValidation(displayName);
                }
            }
        }

        public ColumnsHtmlTag()
        {
            _columns = new Cache.ColumnsCache();
            _codes = new Cache.CodeCache();
        }

        public override HtmlTag GetLabel(FormGroupLayout layout)
        {
            if (_column == null)
                return base.GetLabel(layout);
            if (!string.IsNullOrEmpty(Metadata.DisplayName))
                _column.DisplayName = Metadata.DisplayName;

            string columnLabel = GetDefaultDisplayName();

            var label = ComposeLabelTag(layout, columnLabel);
            if (string.IsNullOrEmpty(_column.Tooltips))
                return label;
            else
                return label.Attr("uib-tooltip", _column.Tooltips);
        }

        public override HtmlTag GetInput(FormGroupType type, string value, Dictionary<string, string> selectOptions = null, bool disable = false)
        {
            var attributeDefinedDropdownInput = SetDropdownAttribute(type, selectOptions);
            if (attributeDefinedDropdownInput != null)
                return attributeDefinedDropdownInput;

            if (_column == null || type != FormGroupType.Default)
            {
                return base.GetInput(type, value, selectOptions, disable);
            }

            if (_column.DataType == FieldType.Int32 || _column.DataType == FieldType.Decimal)
            {
                // For number type input
                return base.GetInput(FormGroupType.Number, null, null, disable);
            }
            if (_column.DataType == FieldType.String && !string.IsNullOrEmpty(_column.DropdwonKey))
            {
                HtmlTag input = ProcessDropdownFor(_column.ColumnName, _column.DropdwonKey, _column.DropdwonCascade, false);
                if (input != null)
                    return input;
            }
            if (_column.DataType == FieldType.String && !string.IsNullOrEmpty(_column.ValueforTable))
            {
                HtmlTag input = ProcessValueFor();
                if (input != null)
                    return input;
            }
            return base.GetInput(type, value, selectOptions, disable);
        }

        private HtmlTag SetDropdownAttribute(FormGroupType type, Dictionary<string, string> selectOptions)
        {
            if (selectOptions != null && selectOptions.Count > 0)
                return null;

            if (_metadata.AdditionalValues.ContainsKey(ConstantString.ADDITIONAL_VALUES_CODEMAP))
            {
                string codeMap = _metadata.AdditionalValues[ConstantString.ADDITIONAL_VALUES_CODEMAP].ToString();
                string codeMapFor = null;
                bool emptyField = false;
                if (_metadata.AdditionalValues.ContainsKey(ConstantString.ADDITIONAL_VALUES_CODEMAP_FOR))
                    codeMapFor = _metadata.AdditionalValues[ConstantString.ADDITIONAL_VALUES_CODEMAP_FOR].ToString();
                if (_metadata.AdditionalValues.ContainsKey(ConstantString.ADDITIONAL_VALUES_CODEMAP_EMPTY_SELECTION))
                    emptyField = true;
                return ProcessDropdownFor(Name, codeMap, codeMapFor, emptyField);

            }
            return null;
        }

        public override void ApplyValidationToInput(HtmlTag input)
        {
            base.ApplyValidationToInput(input);
            if (_column != null 
                && !_column.Nullable 
                && !input.HasAttr(ConstantString.VALIDATION_REQUIRED_FIELD))
            {
                AddRquiredAttribute(input);
            }
        }

        protected override string GetDefaultDisplayName()
        {
            if (_column != null && !string.IsNullOrEmpty(_column.DisplayName))
                return _column.DisplayName;
            return base.GetDefaultDisplayName();
        }

        protected HtmlTag ProcessValueFor()
        {
            HtmlTag input = null;
            if (!string.IsNullOrEmpty(_column.ValueforValue) && !string.IsNullOrEmpty(_column.ValueforKey))
            {
                Dictionary<string, string> options = GetTableOptions(_column.ValueforTable, _column.ValueforKey, _column.ValueforValue, _column.Valuefor);
                if (options != null && options.Count > 0)
                {
                    input = base.GetInput(FormGroupType.Dropdown, null, options);                    
                }
            }
            return input;
        }

        protected virtual Dictionary<string, string> GetTableOptions(string valueforTable, string valueforKey, string valueforValue, string valueFor)
        {
            return new Dictionary<string, string>();
        }

        private HtmlTag ProcessDropdownFor(string name, string dropdownKey, string dropdownCascade, bool emptyField)
        {
            HtmlTag input = null;
            if (!string.IsNullOrEmpty(dropdownCascade))
            {
                input = base.GetInput(FormGroupType.Dropdown, null);
                input.Attr("dropdown-cascade", GetPrefixDropdownCascade(dropdownCascade, input));
                var optionsName = "codedetail" + name;
                input.Attr("ng-options", "key as value for (key, value) in {0}"
                    .FormatWith(optionsName));
                input.Attr("code-map", dropdownKey);
                input.Attr("cascade-options", optionsName);
            }
            else
            {

                Dictionary<string, string> options = _codes.GetDetails(dropdownKey);
                if (emptyField)
                {
                    var optionlist = options.ToList();
                    optionlist.Insert(0, new KeyValuePair<string, string>("", "---請選擇---"));
                    options = optionlist.ToDictionary(k => k.Key, v => v.Value);
                }
                input = base.GetInput(FormGroupType.Dropdown, null, options);
            }

            return input;
        }

        private string GetPrefixDropdownCascade(string fieldName, HtmlTag input)
        {
            fieldName = JsonHelper.ConvertToCamelCase(fieldName);
            string ngModel = input.Attr("ng-model");
            string[] prefixeName = ngModel.Split('.');
            prefixeName[prefixeName.Length - 1] = fieldName;

            return string.Join(".", prefixeName);
        }

        private void AddPatternValidation(string displayName)
        {
            if (_column.DataType == FieldType.String && !string.IsNullOrEmpty(_column.DisplayFormat))
            {
                _validations.Rules.Add(ConstantString.VALIDATION_PATTERN,
                    new
                    {
                        message = ConstantString.VALIDATION_PATTERN_MESSAGE.FormatWith(displayName),
                        parameters = _column.DisplayFormat
                    });
            }
        }

        private void AddNumberRangeValaidation(string displayName)
        {
            if (_column.DataType == FieldType.Decimal || _column.DataType == FieldType.Int32)
            {
                if (_column.MinValue.HasValue)
                {
                    _validations.Rules.Add(ConstantString.VALIDATION_MIN_VALUE,
                        new
                        {
                            message = ConstantString.VALIDATION_MIN_MESSAGE
                                .FormatWith(displayName, _column.MinValue),
                            parameters = _column.MinValue
                        });
                }
                if (_column.MaxValue.HasValue)
                {
                    _validations.Rules.Add(ConstantString.VALIDATION_MAX_VALUE,
                        new
                        {
                            message = ConstantString.VALIDATION_MAX_MESSAGE
                                .FormatWith(displayName, _column.MaxValue),
                            parameters = _column.MaxValue
                        });
                }
            }
        }

        private void AddStringLengthValidation(string displayName)
        {
            if (_column.DataType == FieldType.String)
            {
                if (_column.StringMaxLength > 0
                    && !_validations.Rules.ContainsKey(ConstantString.VALIDATION_MAXLENGTH_FIELD))
                {
                    _validations.Rules.Add(ConstantString.VALIDATION_MAXLENGTH_FIELD, new
                    {
                        message = ConstantString.VALIDATION_MAXLENGTH_MESSAGE
                            .FormatWith(displayName, _column.StringMaxLength),
                        parameters = _column.StringMaxLength
                    });
                }
                if (_column.StringMinLength.HasValue
                    && !_validations.Rules.ContainsKey(ConstantString.VALIDATION_MINLENGTH_FIELD))
                {
                    _validations.Rules.Add(ConstantString.VALIDATION_MINLENGTH_FIELD, new
                    {
                        message = ConstantString.VALIDATION_MINLENGTH_MESSAGE
                            .FormatWith(displayName, _column.StringMinLength),
                        parameters = _column.StringMinLength
                    });
                }
            }
        }

        private void AddRequiredValidation(string displayName)
        {
            if (!_column.Nullable)
            {
                if (!_validations.Rules.ContainsKey(ConstantString.VALIDATION_REQUIRED_FIELD))
                    _validations.Rules.Add(ConstantString.VALIDATION_REQUIRED_FIELD,
                        new
                        {
                            message = ConstantString.VALIDATION_REQUIRED_MESSAGE
                                .FormatWith(displayName)
                        });
            }
        }
    }
}
