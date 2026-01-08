// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;

namespace Moryx.Tools;

/// <summary>
/// String Extension Class for all string extensions in EnMSTreeAffairs
/// </summary>
public static class StringExtensions
{
    /// <param name="str">The first parameter takes the "this" modifier and specifies the type for which the method is defined.</param>
    extension(string str)
    {
        /// <summary>
        /// Converts the name string to a valid file system name.
        /// </summary>
        /// <param name="replacement">The replacement char or string.</param>
        /// <returns></returns>
        public string ConvertToValidFileSystemName(string replacement = "")
        {
            //Loading invalid characters form system.IO
            var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            //Aggregate invalid with the replacement
            var result = str;
            foreach (var c in invalid)
                result = result.Replace(c.ToString(CultureInfo.InvariantCulture), replacement);

            return result.Replace(" ", "_");
        }

        /// <summary>
        /// Appends the strings and return a new instance.
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="value">The value.</param>
        /// <returns>new string instance.</returns>
        public string Append(string separator, string value)
        {
            ArgumentNullException.ThrowIfNull(str);

            if (str.Length == 0)
            {
                return value ?? str;
            }

            if (!string.IsNullOrEmpty(value))
            {
                return string.Join(separator, str, value);
            }

            return str;
        }

        /// <summary>
        /// Die Funktions stellt sicher, ob mindestens ein Zeichen in dem value eine Buchstabe oder eine Zahl ist
        /// </summary>
        /// <returns></returns>
        public bool IsAlphaNumeric()
        {
            return !string.IsNullOrEmpty(str) && str.ToCharArray().Any(c => char.IsLetter(c) || char.IsNumber(c));
        }

        /// <summary>
        /// Reduce string to shorter preview which is optionally ended by some string (...).
        /// </summary>
        /// <param name="count">Length of returned string including endings.</param>
        /// <param name="endings">optional edings of reduced text</param>
        /// <example>
        /// string description = "This is very long description of something";
        /// string preview = description.Reduce(20,"...");
        /// produce -> "This is very long..."
        /// </example>
        /// <returns></returns>
        public string Reduce(int count, string endings)
        {
            if (count < endings.Length)
                throw new Exception("Failed to reduce to less then endings length.");

            int sLength = str.Length;
            int len = sLength;

            if (endings != null)
                len += endings.Length;

            if (count > sLength)
                return str; //it's too short to reduce

            str = str.Substring(0, sLength - len + count);
            if (endings != null)
                str += endings;

            return str;
        }
    }
}