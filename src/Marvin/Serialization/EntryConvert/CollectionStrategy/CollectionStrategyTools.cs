using System.Collections.Generic;
using System.Linq;
using Marvin.Tools;

namespace Marvin.Serialization
{
    /// <summary>
    /// Tools for the collection strategies
    /// </summary>
    internal static class CollectionStrategyTools
    {
        /// <summary>
        /// Get name of collection entries
        /// </summary>
        public static string GetEntryName(object item)
        {
            var itemType = item.GetType();

            // Value type do not have names
            if (EntryConvert.ValueOrStringType(itemType))
                return itemType.Name;

            // Check for display name declaration
            var displayName = itemType.GetDisplayName();
            if (!string.IsNullOrWhiteSpace(displayName))
                return displayName;

            // Check if item declares its own version of ToString()
            return itemType.GetMethod(nameof(ToString)).DeclaringType == typeof(object) ? itemType.Name : item.ToString();
        }

        /// <summary>
        /// Create entry for object from collection
        /// </summary>
        public static Entry CreateSub(object item, int index, ICustomSerialization customSerialization)
        {
            var subEntry = EntryConvert.EncodeObject(item, customSerialization);
            subEntry.Key.Name = GetEntryName(item);
            subEntry.Key.Identifier = index.ToString("D");

            return subEntry;
        }

        /// <summary>
        /// Generate a number of keys
        /// </summary>
        public static IEnumerable<string> GenerateKeys(int count)
        {
            return count == 0 ? new string[0] : Enumerable.Range(0, count).Select(i => i.ToString("D"));
        }
    }
}