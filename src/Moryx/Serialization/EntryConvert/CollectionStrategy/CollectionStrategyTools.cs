// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Globalization;
using Moryx.Tools;

namespace Moryx.Serialization;

/// <summary>
/// Tools for the collection strategies
/// </summary>
internal static class CollectionStrategyTools
{
    /// <summary>
    /// Create an <see cref="Entry"/> for the <paramref name="item"/> at <paramref name="index"/> of a collection.
    /// Encoding uses either <paramref name="encodeItem"/> or
    /// <see cref="EntryConvert.EncodeObject(object, ICustomSerialization)"/>.
    /// </summary>
    public static Entry CreateSub(object item, int index, ICustomSerialization customSerialization, Func<object, Entry> encodeItem = null)
    {
        var subEntry = EncodeItem(item, customSerialization, encodeItem);
        // Encoders may encode null items, which keep the name assigned by the encoder
        if (item != null)
        {
            subEntry.DisplayName ??= GetEntryName(item);
            subEntry.Description ??= GetEntryDescription(item);

        }
        subEntry.Identifier = index.ToString("D", CultureInfo.InvariantCulture);

        return subEntry;
    }

    public static Entry EncodeItem(object item, ICustomSerialization customSerialization, Func<object, Entry> encodeItem) =>
        encodeItem != null ? encodeItem(item) : EntryConvert.EncodeObject(item, customSerialization);

    private static string GetEntryName(object item)
    {
        var itemType = item.GetType();

        // Value type do not have names
        if (EntryConvert.ValueOrStringType(itemType))
        {
            return itemType.Name;
        }

        // Check for display name declaration
        var displayName = itemType.GetDisplayName();
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            return displayName;
        }

        // Check if item declares its own version of ToString()
        return itemType.GetMethod(nameof(ToString))!.DeclaringType == typeof(object) ? itemType.Name : item.ToString();
    }

    private static string GetEntryDescription(object item) => item.GetType().GetDescription();

    /// <summary>
    /// Generate a number of keys
    /// </summary>
    public static IEnumerable<string> GenerateKeys(int count)
        => count == 0 ? [] : Enumerable.Range(0, count).Select(i => i.ToString("D", CultureInfo.InvariantCulture));

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
                itemsByKey[i.ToString("D", CultureInfo.InvariantCulture)] = list[i];
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
        // TODO: Remove in next major version once all consumers use unique CREATED identifiers
        // Skip reorder if keys aren't unique (e.g. multiple "CREATED" entries)
        if (newKeyOrder.Count != newKeyOrder.Distinct().Count())
        {
            return false;
        }

        var defaultOrder = GenerateKeys(list.Count)
            .Where(key => !toDelete.Contains(list[int.Parse(key, CultureInfo.InvariantCulture)]))
            .Concat(addedItems.Keys);

        return !newKeyOrder.SequenceEqual(defaultOrder);
    }
}
