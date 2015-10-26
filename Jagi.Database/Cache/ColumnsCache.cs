using Jagi.Database.Models;
using Jagi.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Jagi.Database.Cache
{
    public class ColumnsCache : CacheBase
    {
        private const string REGION_NAME = "COLUMNS";
        private const string SCHEMA_CACHE_TABLES = "_TABLES";
        private const string KEY_FORMAT = "TableName:{0};ColumnNanme:{1}";
        public ColumnsCache() : base(REGION_NAME) {
            _cache = MemoryCache.Default;
        }
        public ColumnsCache(MemoryCache mainCache)
            : base(REGION_NAME)
        {
            _cache = mainCache;
        }

        public void Set(TableSchema column)
        {
            string key = string.Format(KEY_FORMAT, column.TableName, column.ColumnName);
            Set(key, column);
            var tableCache = base.Get<List<string>>(SCHEMA_CACHE_TABLES);
            if (tableCache == null)
                base.Set(SCHEMA_CACHE_TABLES, new List<string> { column.TableName });
            else
            {
                if (!tableCache.Contains(column.TableName))
                    tableCache.Add(column.TableName);
            }
        }

        public void Remove(string tableName, string columnName)
        {
            string key = string.Format(KEY_FORMAT, tableName, columnName);
            Remove(key);
        }

        public TableSchema Get(string tableName, string columnName)
        {
            string key = string.Format(KEY_FORMAT, tableName, columnName);
            var column = base.Get<TableSchema>(key);
            if (column == null)
                column = base.Get<TableSchema>(string.Format(KEY_FORMAT, ConstantString.SCHEMA_DEFAULT_TABLE_NAME, columnName));
            return column;
        }

        /// <summary>
        /// 取回欄位顯示名稱
        /// </summary>
        /// <param name="tableName">資料庫 Table 名稱</param>
        /// <param name="columnName">資料庫欄位名稱</param>
        /// <returns>欄位在 view 中的顯示名稱</returns>
        public string GetDisplayName(string tableName, string columnName)
        {
            var column = Get(tableName, columnName);
            if (column == null)
                column = base.Get<TableSchema>(string.Format(KEY_FORMAT, ConstantString.SCHEMA_DEFAULT_TABLE_NAME, columnName));
            if (column != null)
                return column.DisplayName;

            return string.Empty;
        }

        /// <summary>
        /// 取回 Columns Cache 中，所有 Table names
        /// </summary>
        /// <returns>List of Table Names </returns>
        public List<string> GetTableNames()
        {
            return base.Get<List<string>>(SCHEMA_CACHE_TABLES);
        }

        /// <summary>
        /// 傳入 View Model Type name，判斷是否 Cache 中有此相關的 Table name
        /// 判斷依據為前面的字串符合 table name (如果最後有 s 會忽略掉)；例如：
        /// PatientEditView => Patients 或 Patient
        /// </summary>
        /// <param name="typeName">view model Type name</param>
        /// <returns>Table name</returns>
        public string GetRelativeTableName(string typeName)
        {
            var tableNames = GetTableNames();
            if (tableNames == null || tableNames.Count == 0)
                return string.Empty;

            // 考量 Table Name 可能使用複數形態，但 View 則未必須要這樣表示
            var tableName = tableNames.SingleOrDefault(p =>
                typeName.StartsWith(p)
                || typeName.StartsWith(p.TrimEnd('s')));
            // 7/31 修正做法，因要整合 CKD & KIDIT，主要在 Patient & Start 需要使用複數區隔
            // 因此爾後 CKD 的 View 一定要是： PatientsEditView 這樣的型態！
            // 已經修正 tablename: Patientc' startc' and Examc
            //var tableName = tableNames.SingleOrDefault(p => typeName.StartsWith(p));
            return tableName;
        }

    }
}
