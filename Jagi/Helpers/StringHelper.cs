using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jagi.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// 提供可以直接使用 {0}, {1} 的方式，透過後面參數傳入；如：
        /// "Test {0} is {1}".FormatWith("apple", "orange");
        /// </summary>
        /// <param name="formatString">字串，內可用 {0}, {1} 替代</param>
        /// <param name="args">第一個參數代表{0}，第二個{1}，以此類推</param>
        /// <returns></returns>
        public static string FormatWith(this string formatString, params object[] args)
        {
            return args == null || args.Length == 0 ? formatString : string.Format(formatString, args);
        }

        /// <summary>
        /// 任意與日期相關的字串，轉換成標準 DateTime object，可以是國際日期 ex: 2013-12-01T00:00:00.000Z
        /// 或者 "yyyy/MM/dd", "yyyy-MM-dd", "yyyy/M/dd", "yyyy-M-dd", "yyyy/MM/d", "yyyy-MM-d", "yyyy/M/d", "yyyy-M-d"
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(this string dateString)
        {
            // 處理國際日期： ex: 2013-12-01T00:00:00.000Z
            if (!string.IsNullOrEmpty(dateString) && dateString.Contains("T"))
                dateString = dateString.Split('T')[0];

            string[] dateFormats = { "yyyy/MM/dd", "yyyy-MM-dd", "yyyy/M/dd", "yyyy-M-dd", "yyyy/MM/d", "yyyy-MM-d", "yyyy/M/d", "yyyy-M-d", "dd/MM/yyyy", "d/M/yyyy" };
            DateTime datetime = DateTime.ParseExact(dateString, dateFormats, null, DateTimeStyles.AllowWhiteSpaces);

            return datetime;
        }
    }
}
