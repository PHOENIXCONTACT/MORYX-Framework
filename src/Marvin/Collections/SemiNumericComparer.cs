using System;
using System.Collections.Generic;

namespace Marvin.Collections
{
    /// <summary>
    /// Comparer to order string represented integers by their number (int or string possible)
    /// </summary>
    public class SemiNumericComparer : IComparer<string>
    {
        // Origin: https://stackoverflow.com/questions/6396378/c-sharp-linq-orderby-numbers-that-are-string-and-you-cannot-convert-them-to-int
        /// <inheritdoc />
        public int Compare(string string1, string string2)
        {
            int value1;
            int value2;
            var string1IsNumeric = IsNumeric(string1, out value1);
            var string2IsNumeric = IsNumeric(string2, out value2);

            if (string1IsNumeric && string2IsNumeric)
            {
                if (value1 > value2) return 1;
                if (value1 < value2) return -1;
                if (value1 == value2) return 0;
            }

            if (string1IsNumeric && !string2IsNumeric)
                return -1;

            if (!string1IsNumeric && string2IsNumeric)
                return 1;

            return string.Compare(string1, string2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if the value is an integer
        /// </summary>
        public static bool IsNumeric(object value, out int i)
        {
            try
            {
                i = Convert.ToInt32(value.ToString());
                return true;
            }
            catch (FormatException)
            {
                i = 0;
                return false;
            }
        }
    }
}