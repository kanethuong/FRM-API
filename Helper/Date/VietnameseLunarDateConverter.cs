using System;
using System.Globalization;

namespace kroniiapi.Helper
{
    public static class VietnameseLunarDateConverter
    {
        /// <summary>
        /// Get the solar date from the lunar date
        /// </summary>
        /// <param name="day">the lunar day-of-month</param>
        /// <param name="month">the lunar month</param>
        /// <param name="year">the lunar year</param>
        /// <returns>the solar date</returns>
        public static DateTime LunarDate(int day, int month, int year)
        {
            return new DateTime(year, month, day, new VietnameseCalendar());
        }
    }
}
