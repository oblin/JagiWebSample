using Jagi.Database.Cache;
using Jagi.Helpers;
using System.Collections.Generic;
using System.ComponentModel;

namespace Jagi.Database.Mvc
{
    public static class EntityHelper
    {
        /// <summary>
        /// 將 Entity T 取出 columns cache 的 display name，轉成 dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDisplayName<T>(this T value)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var columns = new ColumnsCache();

            string typeName = value.GetType().Name;
            string tableName = columns.GetRelativeTableName(typeName);
            if (string.IsNullOrEmpty(tableName))
            {
                // 沒有在 columns cache 定義，傳回 humanized result
                return GetHumanizedName(value, result);
            }

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
            {
                var column = columns.Get(tableName, property.Name);
                var displayName = 
                    column == null 
                        ? property.Name 
                        : string.IsNullOrEmpty(column.DisplayName) 
                            ? property.Name 
                            : column.DisplayName;

                result.Add(property.Name, displayName);
            }
            return result;
        }

        private static Dictionary<string, string> GetHumanizedName<T>(T value,
            Dictionary<string, string> dictionary)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
            {
                dictionary.Add(property.Name, property.Name.ReadableName());
            }

            return dictionary;
        }
    }
}