// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using Moryx.Tools;

namespace Moryx.Serialization;

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
        subEntry.DisplayName = GetEntryName(item);
        subEntry.Identifier = index.ToString("D");

        return subEntry;
    }

    /// <summary>
    /// Generate a number of keys
    /// </summary>
    public static IEnumerable<string> GenerateKeys(int count)
    {
        return count == 0 ? Array.Empty<string>() : Enumerable.Range(0, count).Select(i => i.ToString("D"));
    }

    /// <summary>
    /// Build a lookup of all items by their key, excluding deleted items and including added items.
    /// Used by strategies to resolve the final item order during reordering.
    /// </summary>
    public static Dictionary<string, object> BuildItemLookup(IList list, IList toDelete, Dictionary<string, object> addedItems)
    {
        var itemsByKey = new Dictionary<string, object>();
        for (var i = 0; i < list.Count; i++)
        {
            if (!toDelete.Contains(list[i]))
            {
                itemsByKey[i.ToString("D")] = list[i];
            }
        }
        foreach (var (key, value) in addedItems)
        {
            itemsByKey[key] = value;
        }
        return itemsByKey;
    }

    /// <summary>
    /// Check if the new key order differs from the default order (existing keys minus removed, then added).
    /// Returns true if reordering is needed.
    /// </summary>
    public static bool NeedsReordering(IReadOnlyList<string> newKeyOrder, IList list, IList toDelete, Dictionary<string, object> addedItems)
    {
        var defaultOrder = GenerateKeys(list.Count)
            .Where(k => !toDelete.Contains(list[int.Parse(k)]))
            .Concat(addedItems.Keys);

        return !newKeyOrder.SequenceEqual(defaultOrder);
    }
}