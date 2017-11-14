using System;
using Marvin.Tools;
using NUnit.Framework;

namespace Marvin.Tests.Extensions
{
    [TestFixture]
    public class DateTimeExtensionTests
    {
        #region WeekNumbers

        private static readonly object[] WeekNumberCases =
        {
            new object[] {new DateTime(2000, 12, 31), 52},
            new object[] {new DateTime(2001, 1, 1), 1},
            new object[] {new DateTime(2005, 1, 1), 53},
            new object[] {new DateTime(2007, 12, 31), 1},
            new object[] {new DateTime(2008, 12, 29), 1},
            new object[] {new DateTime(2010, 1, 3), 53},
            new object[] {new DateTime(2011, 12, 31), 52},
            new object[] {new DateTime(2012, 1, 1), 52},
            new object[] {new DateTime(2013, 1, 2), 1},
            new object[] {new DateTime(2013, 12, 31), 1},
        };

        [Test, TestCaseSource("WeekNumberCases")]
        public void WeekNumberTest(DateTime dateTime, int result)
        {
            var week = dateTime.WeekNumber();

            Assert.AreEqual(week, result, "Week is not as expected!");
        }

        #endregion

        #region AddWeeks

        private static readonly object[] AddWeekCases =
        {
            new object[] {new DateTime(2015, 7, 1), 5, new DateTime(2015, 8, 5)},
            new object[] {new DateTime(2015, 2, 12), 3, new DateTime(2015, 3, 5)},
            new object[] {new DateTime(2008, 4, 4), 12, new DateTime(2008, 6, 27)},
        };

        [Test, TestCaseSource("AddWeekCases")]
        public void AddWeekTest(DateTime dateTime, int weeksToAdd, DateTime resultDateTime)
        {
            var result = dateTime.AddWeeks(weeksToAdd);

            AssertEqualDateTime(result, resultDateTime);
        }

        #endregion

        #region Start and End of Week

        private static readonly object[] StartEndOfWeekCases =
        {
            new object[] {new DateTime(2015, 12, 8), new DateTime(2015, 12, 7), new DateTime(2015, 12, 13, 23, 59, 59, 999)},
            new object[] {new DateTime(2015, 12, 24), new DateTime(2015, 12, 21), new DateTime(2015, 12, 27, 23, 59, 59, 999)},
        };

        [Test, TestCaseSource("StartEndOfWeekCases")]
        public void StartOfWeekTest(DateTime dateTime, DateTime resultStart, DateTime resultEnd)
        {
            var startOfWeek = dateTime.StartOfWeek();

            AssertEqualDateTime(startOfWeek, resultStart);
        }

        [Test, TestCaseSource("StartEndOfWeekCases")]
        public void EndOfWeekTest(DateTime dateTime, DateTime resultStart, DateTime resultEnd)
        {
            var endOfWeek = dateTime.EndOfWeek();

            AssertEqualDateTime(endOfWeek, resultEnd);
        }

        #endregion

        #region Start and End of Week but with year and calendar week

        private static readonly object[] StartEndOfWeekWithCalendarWeekCases =
        {
            new object[] {2015, 50, new DateTime(2015, 12, 7), new DateTime(2015, 12, 13, 23, 59, 59, 999)},
            new object[] {2015, 51, new DateTime(2015, 12, 14), new DateTime(2015, 12, 20, 23, 59, 59, 999)},
        };

        [Test, TestCaseSource("StartEndOfWeekWithCalendarWeekCases")]
        public void StartOfWeekTest(int year, int calendarweek, DateTime startResult, DateTime endResult)
        {
            var datetime = DateTimeExtensions.StartOfWeek(year, calendarweek);
            AssertEqualDateTime(datetime, startResult);
        }

        [Test, TestCaseSource("StartEndOfWeekWithCalendarWeekCases")]
        public void EndOfWeekTest(int year, int calendarweek, DateTime startResult, DateTime endResult)
        {
            var datetime = DateTimeExtensions.EndOfWeek(year, calendarweek);
            AssertEqualDateTime(datetime, endResult);
        }

        #endregion

        #region Start and End of Month

        private static readonly object[] StartEndOfMonthCases =
        {
            new object[] {new DateTime(2015, 12, 15), new DateTime(2015, 12, 1), new DateTime(2015, 12, 31, 23, 59, 59, 999)},
            new object[] {new DateTime(2015, 6, 12), new DateTime(2015, 6, 1), new DateTime(2015, 6, 30, 23, 59, 59, 999)},
        };

        [Test, TestCaseSource("StartEndOfMonthCases")]
        public void StartOfMonthTest(DateTime dateTime, DateTime resultStart, DateTime resultEnd)
        {
            var startOfMonth = dateTime.StartOfMonth();
            AssertEqualDateTime(startOfMonth, resultStart);
        }

        [Test, TestCaseSource("StartEndOfMonthCases")]
        public void EndOfMonthTest(DateTime dateTime, DateTime resultStart, DateTime resultEnd)
        {
            var endOfMonth = dateTime.EndOfMonth();
            AssertEqualDateTime(endOfMonth, resultEnd);
        }

        #endregion
        
        #region NumberOfCalendarWeeksForYear

        [Test]
        [TestCase(2015, 53)]
        [TestCase(2016, 52)]
        [TestCase(2017, 52)]
        public void GetNumberOfCalendarWeeksForYearTest(int year, int resultWeeks)
        {
            var numberOfWeeks = DateTimeExtensions.GetNumberOfCalendarWeeksForYear(year);

            Assert.AreEqual(numberOfWeeks, resultWeeks);
        }

        #endregion

        #region DIN EN 60062

        [TestCase(1973, 1, ExpectedResult = "D1")]
        [TestCase(1993, 1, ExpectedResult = "D1")]
        [TestCase(2013, 1, ExpectedResult = "D1")]
        [TestCase(2033, 1, ExpectedResult = "D1")]
        [TestCase(1978, 3, ExpectedResult = "K3")]
        [TestCase(1998, 3, ExpectedResult = "K3")]
        [TestCase(2018, 3, ExpectedResult = "K3")]
        [TestCase(1989, 11, ExpectedResult = "XN")]
        [TestCase(2009, 11, ExpectedResult = "XN")]
        [TestCase(2029, 5, ExpectedResult = "X5")]
        [TestCase(2029, 11, ExpectedResult = "XN")]
        [TestCase(2029, 10, ExpectedResult = "XO")]
        [TestCase(2029, 12, ExpectedResult = "XD")]
        public string Din60062Test(int year, int month)
        {
            return new DateTime(year, month, 1).ToDin60062().ToString();
        }

        [TestCase(1969, 5)]
        public void Din60062ThrowsInvalidOperationException(int year, int month)
        {
            Assert.Throws<InvalidOperationException>(() => new DateTime(year, month, 1).ToDin60062());
        }

        #endregion

        private void AssertEqualDateTime(DateTime a, DateTime b)
        {
            Assert.AreEqual(a.Day, b.Day);
            Assert.AreEqual(a.Hour, b.Hour);
            Assert.AreEqual(a.Minute, b.Minute);
            Assert.AreEqual(a.Millisecond, b.Millisecond);
        }
    }
}