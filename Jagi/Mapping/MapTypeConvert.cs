using AutoMapper;
using Jagi.Helpers;
using System;

namespace Jagi.Mapping
{
    /// <summary>
    /// 設定 AutoMaper 將 Nullabl integer 轉換成數字
    /// </summary>
    public class NullableIntToString : TypeConverter<int? ,string>
    {
        protected override string ConvertCore(int? source)
        {
            if (source.HasValue)
                return ((int)source).ToString();
            else
                return string.Empty;
        }
    }

    /// <summary>
    /// 設定 AutoMaper 將文字轉換成 Nullabl decimal
    /// </summary>
    public class StringToDecimalNull : TypeConverter<string, decimal?>
    {
        protected override decimal? ConvertCore(string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            else
                return System.Convert.ToDecimal(source);
        }
    }

    /// <summary>
    /// 設定 AutoMaper 將文字轉換成 Nullabl DateTime
    /// </summary>
    public class StringToDateTimeNull : TypeConverter<string, DateTime?>
    {
        protected override DateTime? ConvertCore(string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            else
                return source.ConvertToDateTime();
        }
    }
}
