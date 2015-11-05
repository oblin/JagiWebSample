using Jagi.Interface;
using Jagi.Mapping;
using System.ComponentModel.DataAnnotations;

namespace JagiWebSample.Areas.Admin.Models
{
    public class TableSchemaEditView : IMapFrom<TableSchema>
    {
        public int Id { get; set; }

        [Required, StringLength(50), Display(Name = " 資料表名稱")]
        public string TableName { get; set; }

        [Required, StringLength(50), Display(Name = "欄位名稱")]
        public string ColumnName { get; set; }

        [Display(Name = "資料型態")]
        public FieldType DataType { get; set; }

        //[Display(Name = "對應型態 ")]
        //public string DataTypeName { get; set; }

        [Display(Name = "允許空值")]
        public bool Nullable { get; set; }

        [Display(Name = "字串最大長度")]
        public int StringMaxLength { get; set; }

        [Display(Name = "字串最小長度")]
        public int? StringMinLength { get; set; }

        public int Precision { get; set; }
        public int Scale { get; set; }

        [Display(Name = "顯示名稱")]
        public string DisplayName { get; set; }
        [Display(Name = "正規式檢查")]
        public string DisplayFormat { get; set; }

        [Display(Name = "限制最小值")]
        public decimal? MinValue { get; set; }
        [Display(Name = "限制最大值")]
        public decimal? MaxValue { get; set; }
        public decimal? WarningMinValue { get; set; }
        public decimal? WarningMaxValue { get; set; }

        public string Tooltips { get; set; }
        public string FromTable { get; set; }

        [Display(Name = "Code DropdwonKey")]
        public string DropdwonKey { get; set; }
        [Display(Name = "Code DropdwonCascade")]
        public string DropdwonCascade { get; set; }

        [StringLength(20), Display(Name = "對應資料表")]
        public string ValueforTable { get; set; }
        [StringLength(20), Display(Name = "對應KEY欄位")]
        public string ValueforKey { get; set; }
        [StringLength(20), Display(Name = "對應的 Value 欄位")]
        public string ValueforValue { get; set; }
        [StringLength(20), Display(Name = "自動帶出欄位名稱")]
        public string Valuefor { get; set; }
    }

    /// <summary>
    /// Dropdown button only
    /// </summary>
    public class SchemaTablesView
    {
        public string CatalogName { get; set; }
        public string Name { get; set; }
        public string SchemaName { get; set; }
    }
}