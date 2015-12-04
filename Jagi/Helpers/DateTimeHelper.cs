using System;
using System.Collections.Generic;
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
    }
}
