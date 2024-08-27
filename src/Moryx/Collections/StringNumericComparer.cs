// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Moryx.Collections
{
    /// <summary>
    /// Comparer to order string represented integers as well as mixed strings with trailing numbers using IComparer interface
    /// For the comparison the string is interpreted as "{text}{number}" and the text portion is trimmed.
    /// It will sort null or numbers without text in the beginning. Text without suffix comes before the same text with suffix. Otherwise
    /// text and numbers are compared using their respective implementations of IComparable
    /// </summary>
    /// <remarks>
    /// https://stackoverflow.com/questions/6396378/c-sharp-linq-orderby-numbers-that-are-string-and-you-cannot-convert-them-to-int
    /// </remarks>
    public class StringNumericComparer : IComparer<string>
    {
        /// <inheritdoc />
        public int Compare(string a, string b)
        {
            // First check for null or simple equality
            if (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b))
                return 0;
            if (string.IsNullOrWhiteSpace(a))
                return -1;
            if (string.IsNullOrWhiteSpace(b))
                return 1;
            if (a == b)
                return 0;

            // Now detect text and numeric part of the string
            var regex = new Regex(@"(?<text>(?:[A-Za-z]+\s*)+)?(?<number>\d+)?");
            var matchA = regex.Match(a);
            var matchB = regex.Match(b);
            // Extract values
            var hasTextA = matchA.Groups["text"].Success;
            var textA = hasTextA ? matchA.Groups["text"].Value.Trim() : null;
            var hasTextB = matchB.Groups["text"].Success;
            var textB = hasTextB ? matchB.Groups["text"].Value.Trim() : null;
            var hasNumberA = matchA.Groups["number"].Success;
            var numberA = hasNumberA ? int.Parse(matchA.Groups["number"].Value) : 0;
            var hasNumberB = matchB.Groups["number"].Success;
            var numberB = hasNumberB ? int.Parse(matchB.Groups["number"].Value) : 0;

            // Both sections contain text
            if (hasTextA & hasTextB)
            {
                // Compare texts first
                var textCompare = string.Compare(textA, textB, StringComparison.OrdinalIgnoreCase);
                if (textCompare != 0)
                    return textCompare;

                // Compare numeric suffix
                if (hasNumberA & hasNumberB)
                    return numberA.CompareTo(numberB);
                if (hasNumberA)
                    return 1;
                if (hasNumberB)
                    return -1;
                return 0;
            }
            // Only the first hast text
            if (hasTextA)
            {
                return 1;
            }
            // Only the second has text
            if (hasTextB)
                return -1;
            // None has text
            return numberA.CompareTo(numberB);
        }
    }
}
