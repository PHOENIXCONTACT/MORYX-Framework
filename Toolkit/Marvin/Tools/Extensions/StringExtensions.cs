using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Marvin.Tools
{
    /// <summary>
    /// String Extension Class for all string extensions in EnMSTreeAffairs
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts the name string to a valid file system name.
        /// </summary>
        /// <param name="str">The first parameter takes the "this" modifier and specifies the type for which the method is defined.</param>
        /// <param name="replacement">The replacement char or string.</param>
        /// <returns></returns>
        public static string ConvertToValidFileSystemName(this string str, string replacement = "")
        {
            //Loading invalid characters form system.IO
            var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            //Aggregate invalid with the replacement
            string result = str;
            foreach (char c in invalid)
            {
                result = result.Replace(c.ToString(CultureInfo.InvariantCulture), replacement);
            }

            return result.Replace(" ", "_");
        }

        /// <summary>
        /// Appends the strings and return a new instance.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="value">The value.</param>
        /// <returns>new string instance.</returns>
        public static string Append(this string s, string separator, string value)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (s.Length == 0)
            {
                return value ?? s;
            }

            if (!string.IsNullOrEmpty(value))
            {
                return string.Join(separator, s, value);
            }
            
            return s;
        }


        /// <summary>
        /// Die Funktions stellt sicher, ob mindestens ein Zeichen in dem value eine Buchstabe oder eine Zahl ist
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsAlphaNumeric(this string value)
        {
            return !string.IsNullOrEmpty(value) && value.ToCharArray().Any(c => char.IsLetter(c) || char.IsNumber(c));
        }

        /// <summary>
        /// Reduce string to shorter preview which is optionally ended by some string (...).
        /// </summary>
        /// <param name="s">string to reduce</param>
        /// <param name="count">Length of returned string including endings.</param>
        /// <param name="endings">optional edings of reduced text</param>
        /// <example>
        /// string description = "This is very long description of something";
        /// string preview = description.Reduce(20,"...");
        /// produce -> "This is very long..."
        /// </example>
        /// <returns></returns>
        public static string Reduce(this string s, int count, string endings)
        {
            if (count < endings.Length)
                throw new Exception("Failed to reduce to less then endings length.");

            int sLength = s.Length;
            int len = sLength;

            if (endings != null)
                len += endings.Length;

            if (count > sLength)
                return s; //it's too short to reduce

            s = s.Substring(0, sLength - len + count);
            if (endings != null)
                s += endings;

            return s;
        }
    }
}