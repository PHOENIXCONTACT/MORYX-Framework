using System.Collections.Generic;
using System.Linq;

namespace Moryx.Tools
{
    /// <summary>
    /// Extension methods for dictionaries
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Determines whether the specified dictionary contains the given keys.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the dictionary contains the given keys; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsKeys(this IDictionary<string, string> dictionary, params string[] keys)
        {
            return dictionary.Any(entry => keys.Contains(entry.Key));
        }
    }
}