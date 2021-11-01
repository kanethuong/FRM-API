using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.Helper.Timetable
{
    public static class TimetableHelper
    {
        /// <summary>
        /// Calculates number of business days, taking into account:
        ///  - weekends (Saturdays and Sundays)
        ///  - bank holidays in the middle of the week
        /// </summary>
        /// <param name="firstDay">First day in the time interval</param>
        /// <param name="lastDay">Last day in the time interval</param>
        /// <param name="bankHolidays">List of bank holidays excluding weekends</param>
        /// <returns>Number of business days during the 'span'</returns>    
        public static int BusinessDaysUntil( DateTime firstDay, DateTime lastDay, List<DateTime> Holidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);
            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount*7)
            {
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7) // Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)    // Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7) // Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }
            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;
            // subtract the number of bank holidays during the time interval
            foreach (DateTime bankHoliday in Holidays)
            {
                DateTime bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }
            return businessDays;
        }
        /// <summary>
        /// Get the First Day of the week of the input day
        /// </summary>
        /// <param name="date"></param>
        /// <returns>First day of the Week</returns>
        public static DateTime FirstDayOfWeek(DateTime date)
        {
            var m = (date.DayOfWeek == DayOfWeek.Sunday ? (DayOfWeek)7 : date.DayOfWeek) - DayOfWeek.Monday;
            var Mon = date.AddDays(( - m));
            return Mon;
        }
        /// <summary>
        /// Get the Last Day of the week of the input day
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Last day of the Week</returns>
        public static DateTime LastDayOfWeek(DateTime date)
        {
            var s = (date.DayOfWeek == DayOfWeek.Sunday ? (DayOfWeek)7 : date.DayOfWeek) - (DayOfWeek)7;
            var Sun = date.AddDays(( - s));
            System.Console.WriteLine(Sun);
            return Sun;
        }
        /// <summary>
        /// Get the Next Monday Day of the week of the input day
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Next Monday of the Week</returns>
        public static DateTime NextMonday(DateTime date)
        {
            var m = (date.DayOfWeek == DayOfWeek.Sunday ? (DayOfWeek)7 : date.DayOfWeek) - DayOfWeek.Monday;
            var Mon = date.AddDays(( - m));
            var NextMonday = Mon.AddDays(7);
            return NextMonday;
        }
    }
}