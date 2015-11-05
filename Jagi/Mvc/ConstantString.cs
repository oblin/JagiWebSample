namespace Jagi.Mvc
{
    public class ConstantString
    {
        #region For Table Schema Controller

        public const string SCHEMA_DEFAULT_TABLE_NAME = "_DEFAULT";

        #endregion For Table Schema Controller

        #region For Jagi.Mvc.Razor

        public const string LABEL_HTML_ATTRIBUTES = "labelHtmlAttr";
        public const string EXCLUDED_ATTRIBUTE = "Excluded";
        public const string NEWLINE_ATTRIBUTE = "Newline";
        public const string REQUIREDOR_ATTRIBUTE = "RequiredOr";
        public const string GRID_COLUMN = "col-xs-";

        public const string ADDITIONAL_VALUES_NUMERIC_STYLES = "_STYLES";
        public const string ADDITIONAL_VALUES_CODE_VALUE = "_CODEMAP_VALUE";
        public const string ADDITIONAL_VALUES_CODEMAP = "_CODEMAP";
        public const string ADDITIONAL_VALUES_CODE_OPTIONS = "_CODEMAP_OPTIONS";
        public const string ADDITIONAL_VALUES_CODEMAP_FOR = "_CODEMAP_FOR";

        public const string DATATYPE_NAME_VALUEFOR = "Valuefor";
        public const string DATATYPE_NAME_VALUEFORKEY = "ValueforKey";
        public const string DATATYPE_NAME_VALUEFORVALUE = "ValueforValue";
        public const string DATATYPE_NAME_VALUEFORTABLE = "ValueforTable";

        #endregion For Jagi.Mvc.Razor

        #region For Validations
        public const string VALIDATION_REQUIRED_FIELD = "required";
        public const string VALIDATION_REQUIRED_MESSAGE = "【{0}】必須要輸入";

        public const string VALIDATION_MINLENGTH_FIELD = "minlength";
        public const string VALIDATION_MINLENGTH_MESSAGE = "【{0}】最小長度為：{1}";

        public const string VALIDATION_MAXLENGTH_FIELD = "maxlength";
        public const string VALIDATION_MAXLENGTH_MESSAGE = "【{0}】最大長度不可超過：{1}";

        public const string VALIDATION_MIN_VALUE = "min";
        public const string VALIDATION_MIN_MESSAGE = "【{0}】數值最小必須大於：{1}";

        public const string VALIDATION_MAX_VALUE = "max";
        public const string VALIDATION_MAX_MESSAGE = "【{0}】數值最大必須小於：{1}";

        public const string VALIDATION_PATTERN = "pattern";
        public const string VALIDATION_PATTERN_MESSAGE = "{0} 格式錯誤，請確認填寫正確格式";
        #endregion For Validations
    }
}