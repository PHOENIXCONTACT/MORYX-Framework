using System;
using System.Collections.Generic;

namespace Marvin.Tools
{
    /// <summary>
    /// Extensions for the IList 
    /// </summary>
    public static class ListExtensions
    {
        private static readonly Random Rng = new Random();

        /// <summary>
        /// Shuffles the list 
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}