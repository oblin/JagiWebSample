using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jagi.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime FirstDateOfMonth(this DateTime date){
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime LastDateOfMonth(this DateTime date)
        {
            DateTime firstDate = new DateTime(date.Year, date.Month, 1);
            return firstDate.AddMonths(1).AddDays(-1);
        }

        public static void GetYearAndWeek(this DateTime date,
            out int year, out int week, out DateTime weekStartDate, out DateTime weekEndDate)
        {
            DateTime computed;
            if (date == null)
                computed = DateTime.Now;
            else
                computed = (DateTime)date;

            year = computed.Year;
            week = GetWeekOfYear(computed);
            GetWeekStartEndDate(year, week, out weekStartDate, out weekEndDate);
        }

        public static DateTime GetWeekStartDate(this DateTime date)
        {
            int year, week; DateTime weekStartDate, weekEndDate;
            date.GetYearAndWeek(out year, out week, out weekStartDate, out weekEndDate);

            return weekStartDate;
        }

        public static int GetWeekOfYear(DateTime dt)
        {
            GregorianCalendar GetWeek = new GregorianCalendar();
            return GetWeek.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        public static void GetWeekStartEndDate(int year, int week,
        out DateTime startDate, out DateTime endDate)
        {
            //設定年
            DateTime firstDay = new DateTime(year, 1, 1);

            int add = 0;

            switch (firstDay.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    add = 0;
                    break;
                case DayOfWeek.Tuesday:
                    add = -1;
                    break;
                case DayOfWeek.Wednesday:
                    add = -2;
                    break;
                case DayOfWeek.Thursday:
                    add = -3;
                    break;
                case DayOfWeek.Friday:
                    add = -4;
                    break;
                case DayOfWeek.Saturday:
                    add = -5;
                    break;
                case DayOfWeek.Sunday:
                    add = -6;
                    break;
            }

            DateTime result1 = new DateTime(year, 1, 1).AddDays((week - 1) * 7).AddDays(add);
            DateTime result2 = new DateTime(year, 1, 1).AddDays((week * 7) - 1).AddDays(add);

            startDate = result1;
            endDate = result2;
        }

        public static int DaysBetween(this DateTime d1, DateTime d2)
        {
            TimeSpan span = d2.Subtract(d1);

            return Convert.ToInt16(Math.Abs(span.TotalDays));
        }

        public static string ConvertToChineseDate(this DateTime date)
        {
            try
            {
                int year = Convert.ToInt16(date.AddYears(-1911).Year);

                string result = year.ToString().PadLeft(3, '0') + "/"
                    + date.Month.ToString().PadLeft(2, '0') + "/"
                    + date.Day.ToString().PadLeft(2, '0');

                //string result = Convert.ToInt16(date.AddYears(-1911).Year) + date.ToString("/MM/dd");

                return result;
            }
            catch (ArgumentOutOfRangeException e)
            {
                return string.Empty;
            }
        }

        public static string ConvertToChineseDate(string sDate)
        {
            var date = Convert.ToDateTime(sDate);
            return ConvertToChineseDate(date);
        }
    }
}
