using Jagi.Database.Models;
using Jagi.Interface;
using Jagi.Helpers;
using Jagi.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Jagi.Database.Cache
{
    public class InitColumnCache : IRunAtStartup
    {
        private DataContext _context;
        private ColumnsCache _cache;

        public InitColumnCache(DataContext context)
        {
            _context = context;
            _cache = new ColumnsCache(MemoryCache.Default);
        }

        public void Execute()
        {
            var columns = _context.TableSchema.AsNoTracking().ToList();
            foreach (var column in columns)
            {
                if (string.IsNullOrEmpty(column.DisplayName))
                {
                    var defaultColumn = _context.TableSchema.SingleOrDefault(p =>
                        p.TableName == ConstantString.SCHEMA_DEFAULT_TABLE_NAME
                        && p.ColumnName == column.ColumnName);
                    if (defaultColumn != null)
                        defaultColumn.CopyTo(column, new string[] { "TableName" });
                }
                _cache.Set(column);
            }
        }
    }
}
