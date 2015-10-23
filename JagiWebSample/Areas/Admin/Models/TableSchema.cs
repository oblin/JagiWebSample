using Jagi.Interface;
using Jagi.Mapping;
using System.ComponentModel.DataAnnotations;

namespace JagiWebSample.Areas.Admin.Models
{
    public class TableSchema : Entity, IMapFrom<TableSchemaEditView>
    {
        [StringLength(50)]
        public string TableName { get; set; }

        [StringLength(50)]
        public string ColumnName { get; set; }

        public FieldType DataType { get; set; }

        [StringLength(50)]
        public string DataTypeName { get; set; }

        #region For DropDown List

        [StringLength(20)]
        public string FromTable { get; set; }

        [StringLength(20)]
        public string DropdwonKey { get; set; }

        [StringLength(20)]
        public string DropdwonValue { get; set; }

        [StringLength(20)]
        public string DropdwonCascade { get; set; }

        #endregion For DropDown List

        public bool Nullable { get; set; }
        public int StringMaxLength { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }

        [StringLength(200)]
        public string DisplayName { get; set; }

        [StringLength(50)]
        public string DisplayFormat { get; set; }

        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? WarningMinValue { get; set; }
        public decimal? WarningMaxValue { get; set; }

        [StringLength(200)]
        public string Tooltips { get; set; }
    }
}