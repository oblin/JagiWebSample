using Jagi.Interface;
using Jagi.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JagiWebSample.Areas.Admin.Models
{
    public class TableSchemaGroupListView
    {
        public string TableName { get; set; }
        public IEnumerable<TableSchemaListView> ColumnList { get; set; }
    }

    public class TableSchemaGroupView
    {
        public string TableName { get; set; }
        public IList<string> TableNames { get; set; }
        public TableSchemaListView Schema { get; set; }
    }

    public class TableSchemaListView : IMapFrom<TableSchema>
    {
        public int Id { get; set; }
        public string ColumnName { get; set; }
        public FieldType DataType { get; set; }
        public string DisplayName { get; set; }
        public bool Nullable { get; set; }
        public string DropdwonKey { get; set; }
        public string DataTypeName { get; set; }
        public string StringMaxLength { get; set; }
    }
}