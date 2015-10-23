using System;
using System.Collections.Generic;
using System.Linq;

namespace Jagi.Helpers
{
    public static class TypeHelper
    {
        /// <summary>
        /// 只適用於 Dictionary<string, List<string>> 會將 string list 轉成 string
        /// </summary>
        public static string DictionaryListValueToString(this Dictionary<string, object> dictionary, string key)
        {
            string result = string.Empty;
            if (dictionary.Keys.Contains(key))
            {
                var list = dictionary[key];
                if (list.GetType().Equals(typeof(List<string>)))
                    result = String.Join(";", list as List<string>);
            }

            return result;
        }

        public static bool IsNumericOrNull(this Type type)
        {
            if (type.IsNumeric()
                || type.Equals(typeof(int?))
                || type.Equals(typeof(decimal?))
                || type.Equals(typeof(float?))
                || type.Equals(typeof(double?))
                )
                return true;

            return false;
        }

        public static bool IsNumeric(this Type type)
        {
            if (type.Equals(typeof(int))
                || type.Equals(typeof(decimal))
                || type.Equals(typeof(float))
                || type.Equals(typeof(double))
                )
                return true;

            return false;
        }

        public static bool HasFloatingPoint(this Type type)
        {
            if (type.Equals(typeof(decimal)) || type.Equals(typeof(decimal?))
                || type.Equals(typeof(float)) || type.Equals(typeof(float?))
                || type.Equals(typeof(double)) || type.Equals(typeof(double?))
                )
                return true;

            return false;
        }
    }
}