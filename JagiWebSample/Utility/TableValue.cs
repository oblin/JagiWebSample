using Jagi.Helpers;
using JagiWebSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JagiWebSample.Utility
{
    public class TableValue
    {
        private DataContext _context;
        public TableValue(DataContext context)
        {
            _context = context;
        }

        public Dictionary<string, string> GetCodeDetail(string tableName, string keyFieldName, string valueFieldName, string value)
        {
            Dictionary<string, string> result = null;

            if (!string.IsNullOrEmpty(value))
            {
                int tmp;
                string sql = string.Empty;
                if (Int32.TryParse(value, out tmp))
                    sql = "SELECT Cast({0} as varchar(50)) as [Key], {1} as [Value] FROM {2} WHERE {0} = {3}"
                        .FormatWith(keyFieldName, valueFieldName, tableName, value);
                else
                    sql = "SELECT Cast({0} as varchar(50)) as [Key], {1} as [Value] FROM {2} WHERE {0} = '{3}'"
                    .FormatWith(keyFieldName, valueFieldName, tableName, value);

                result = _context.Database.SqlQuery<CodeMappingTable>(sql)
                                .ToDictionary(k => k.Key, v => v.Value);
            }
            else
            {
                string sql = "SELECT {0} as [Key], {1} as [Value] FROM {2}"
                            .FormatWith(keyFieldName, valueFieldName, tableName);
                result = _context.Database.SqlQuery<CodeMappingTable>(sql)
                                .ToDictionary(k => k.Key, v => v.Value);
            }
            return result;
        }

        class CodeMappingTable
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}