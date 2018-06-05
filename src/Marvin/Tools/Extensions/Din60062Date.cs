using System;

namespace Marvin.Tools
{
    /// <summary>
    /// This value type represents a date in the DIN 60062 format.  Every DateTime
    /// DIN EN 60062 (labeling of resistors and capacitors) specified in paragraph 5 a simple system of letters and numbers for coding of the manufacturing date 
    /// </summary>
    public struct Din60062Date
    {
        internal Din60062Date(char year, char month, DateTime origin) : this()
        {
            Year = year;
            Month = month;
            Origin = origin;
        }

        /// <summary>
        /// Year character of the DIN 60062 characters
        /// </summary>
        public char Year { get; private set; }

        /// <summary>
        /// Month character of the DIN 60062 characters
        /// </summary>
        public char Month { get; private set; }

        /// <summary>
        /// Origin date
        /// </summary>
        public DateTime Origin { get; private set; }

        /// <summary>
        /// Returns the DIN 60062 string format [Year]-[Month]
        /// </summary>
        public override string ToString()
        {
            return $"{Year}{Month}";
        }

        /// <summary>
        /// Returns the DIN 60062 format [Year]-[DayOfYear]
        /// </summary>
        public string ToYearAndDays()
        {
            return $"{Year}-{Origin.DayOfYear}";
        }
    }
}