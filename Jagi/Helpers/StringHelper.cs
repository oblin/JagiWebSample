using Humanizer;
using System;
using System.Globalization;

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

        public static string ReadableName(this string value)
        {
            return value.Humanize(LetterCasing.Title);
        }

        public static string FixedLength(this string value, int length)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > length)
                return value.Truncate(length);

            return value;
        }

        /// <summary>
        /// 判斷字串是否僅有英文與數字
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool isIntergerOrLetter(this string content)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
            return reg1.IsMatch(content);
        }

        public static string FormatDecimal(this decimal? number)
        {
            if (number == null)
                return string.Empty;

            return FormatDecimal((decimal)number);
        }

        public static string FormatDecimal(this decimal number)
        {
            return string.Format("{0:F2}", number);
        }

        public static string FormatDecimal(int p, int q)
        {
            if (p == 0 || q == 0)
                return FormatDecimal(0.00M);

            decimal r = (decimal)p / (decimal)q;
            return FormatDecimal(r);
        }

        public static string FormatDouble(double number)
        {
            return string.Format("{0:F2}", number);
        }

        public static string FormatRateDouble(double p)
        {
            return FormatDouble(p * 100);
        }

        public static string FormatRateDouble(int p, int q)
        {
            if (p == 0 || q == 0)
                return "0.00";

            double result = (double)p / (double)q;
            return FormatDouble(result * 100);
        }

        public static string ToChinese(this bool value)
        {
            if (value)
                return "是";
            else
                return "否";
        }

        public static string ToYN(this bool value)
        {
            if (value)
                return "Y";
            else
                return "N";
        }
    }
}
