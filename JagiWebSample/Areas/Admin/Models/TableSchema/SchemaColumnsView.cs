namespace JagiWebSample.Areas.Admin.Models
{
    public class SchemaColumnsView
    {
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public string DATA_TYPE { get; set; }
        public int CHARACTER_MAXIMUM_LENGTH { get; set; }
        public string COLUMN_DEFAULT { get; set; }
        public string IS_NULLABLE { get; set; }
        public int NUMERIC_PRECISION { get; set; }
        public int NUMERIC_SCALE { get; set; }
    }
}