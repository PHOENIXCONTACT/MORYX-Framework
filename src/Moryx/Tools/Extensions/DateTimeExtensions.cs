// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;

namespace Moryx.Tools;

/// <summary>
/// All extensions for the date time class
/// </summary>
public static class DateTimeExtensions
{
    extension(DateTime date)
    {
        /// <summary>
        /// Returns the calendar week regarding to ISO8601
        /// </summary>
        public int WeekNumber()
        {
            var calender = Thread.CurrentThread.CurrentCulture.Calendar;

            var day = (int)calender.GetDayOfWeek(date);
            return calender.GetWeekOfYear(date.AddDays(4 - (day == 0 ? 7 : day)), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// Returns a DateTime with the date of the start of the week.
        /// </summary>
        public DateTime StartOfWeek(DayOfWeek startOfWeek)
        {
            var diff = date.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return date.AddDays(-1 * diff).Date; // .Date for Time 00:00:00
        }

        /// <summary>
        /// Returns a DateTime with the date of the start of the week.
        /// </summary>
        public DateTime StartOfWeek()
        {
            return date.StartOfWeek(DayOfWeek.Monday);
        }
    }

    /// <summary>
    /// Mondy 00:00 for this calendar week
    /// </summary>
    public static DateTime StartOfWeek(int year, int calendarweek)
    {
        var result = StartOfWeek(new DateTime(year, 1, 1).AddDays((calendarweek - 1) * 7));
        return result;
    }

    /// <summary>
    /// Sunday 23:59 for this calendar week
    /// </summary>
    /// <param name="dateTime">First day of the year - eg 01.01.2013</param>
    /// <returns>First day of this calendar week</returns>
    public static DateTime EndOfWeek(this DateTime dateTime)
    {
        var start = dateTime.StartOfWeek();
        var end = start.AddDays(7).AddTicks(-1); //one week forward and one tick back
        return end;
    }

    /// <summary>
    /// Sunday 24:00 for this calendar week
    /// </summary>
    /// <param name="year"></param>
    /// <param name="calendarweek">Calendar week</param>
    /// <returns>First day of this calendar week</returns>
    public static DateTime EndOfWeek(int year, int calendarweek)
    {
        // Start at first monday and add 7 days for each calendarweek plus extra week and go to the day before
        return StartOfWeek(year, calendarweek).EndOfWeek();
    }

    /// <param name="dateTime">The date time.</param>
    extension(DateTime dateTime)
    {
        /// <summary>
        /// Add weeks to datetime (n * 7).
        /// </summary>
        /// <param name="numberOfWeeks">The number of weeks.</param>
        /// <returns></returns>
        public DateTime AddWeeks(int numberOfWeeks)
        {
            return dateTime.AddDays(numberOfWeeks * 7);
        }

        /// <summary>
        /// Firsts the of month.
        /// </summary>
        /// <returns></returns>
        public DateTime StartOfMonth()
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// Lasts the of month.
        /// </summary>
        /// <returns></returns>
        public DateTime EndOfMonth()
        {
            // overshoot the date by a month
            var dtTo = new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1);

            dtTo = dtTo.AddTicks(-1);

            // return the last day of the month
            return dtTo;
        }
    }

    /// <summary>
    /// Gets Number of Calendarweeks for the year
    /// </summary>
    /// <param name="year">year</param>
    /// <returns></returns>
    public static int GetNumberOfCalendarWeeksForYear(int year)
    {
        return new DateTime(year, 1, 1).DayOfWeek == DayOfWeek.Thursday | (DateTime.IsLeapYear(year) & new DateTime(year, 1, 1).DayOfWeek == DayOfWeek.Wednesday) ? 53 : 52;
    }
}