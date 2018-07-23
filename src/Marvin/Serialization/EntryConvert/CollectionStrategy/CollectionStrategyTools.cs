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
            var itemType = item.GetType();
            // Entry wrapper around the object in the collection
            var subentry = new Entry
            {
                Key = new EntryKey
                {
                    Name = GetEntryName(item),
                    Identifier = index.ToString("D")
                },
                Value = new EntryValue
                {
                    Type = EntryConvert.TransformType(itemType),
                    Current = EntryConvert.ValueOrStringType(itemType) ? item.ToString() : itemType.Name
                }
            };

            // Recursive call for the children
            if (subentry.Value.Type == EntryValueType.Class)
                subentry.SubEntries.AddRange(EntryConvert.EncodeObject(item, customSerialization));

            return subentry;
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