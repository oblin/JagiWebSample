using Jagi.Interface;
using Jagi.Mapping;
using System.ComponentModel.DataAnnotations;

namespace JagiWebSample.Areas.Admin.Models
{
    public class TableSchemaEditView : IMapFrom<TableSchema>
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string TableName { get; set; }

        [Required, StringLength(50)]
        public string ColumnName { get; set; }

        public FieldType DataType { get; set; }
        public string DataTypeName { get; set; }
        public bool Nullable { get; set; }
        public int StringMaxLength { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public string DisplayName { get; set; }
        public string DisplayFormat { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? WarningMinValue { get; set; }
        public decimal? WarningMaxValue { get; set; }

        public string Tooltips { get; set; }
        public string FromTable { get; set; }
        public string DropdwonKey { get; set; }
        public string DropdwonValue { get; set; }
        public string DropdwonCascade { get; set; }
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