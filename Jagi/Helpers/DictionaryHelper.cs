using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Jagi.Helpers
{
    public static class DictionaryHelper
    {
        /// <summary>
        /// 只適用於 Dictionary<string, List<string>> 會將 value 的 string list，用 sep 分隔，轉成 string
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key">尋找 dictionary 中，符合 key 的 Value</param>
        /// <param name="sep">string list 的分隔符號，預設為 ";"</param>
        /// <returns></returns>   
        public static string ConvertToString(this Dictionary<string, object> dictionary, string key, string sep = ";")
        {
            string result = string.Empty;
            if (dictionary.Keys.Contains(key))
            {
                var list = dictionary[key];
                if (list.GetType().Equals(typeof(List<string>)))
                    result = String.Join(sep, list as List<string>);
            }

            return result;
        }

        /// <summary>
        /// 只適用於 Dictionary<string, List<string>> 會判斷如下：
        /// 1. 如果 key 值存在，則將 value 加入倒 object List 中
        /// 2. 如果 key 值不存在，則新增一筆 Dictionary 資料
        /// </summary>
        /// <param name="key">Dictionary Key</param>
        /// <param name="value">將 value 加入到 Dictionary value List 中</param>
        public static void AppendListString(this Dictionary<string, object> dictionary, string key, string value)
        {
            if (dictionary.Keys.Contains(key))
            {
                var list = dictionary[key];
                if (list.GetType().Equals(typeof(List<string>)))
                    ((List<string>)list).Add(value);
                else
                    throw new Exception("此函數呼叫必須要使用 List<string>！");
            }
            else
            {
                dictionary.Add(key, new List<string> { value });
            }

        }

        public static RouteValueDictionary ToRouteValueDictionary(this Dictionary<string, string> dictionary)
        {
            RouteValueDictionary result = new RouteValueDictionary();
            foreach (var item in dictionary)
                result.Add(item.Key, item.Value);

            return result;
        }

        public static RouteValueDictionary ObjectToRouteValueDictionary(object value)
        {
            RouteValueDictionary dictionary = new RouteValueDictionary();
            if (value != null)
            {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(value))
                {
                    dictionary.Add(descriptor.Name.Replace("_", "-"), descriptor.GetValue(value));
                }
            }

            return dictionary;
        }
    }
}
