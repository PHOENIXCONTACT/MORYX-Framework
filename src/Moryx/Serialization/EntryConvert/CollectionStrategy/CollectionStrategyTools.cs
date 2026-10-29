// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
        // Converters may encode null items, which keep the name assigned by the converter
        if (item != null)
        {
            subEntry.DisplayName ??= GetEntryName(item);
            subEntry.Description ??= GetEntryDescription(item);

        }
        subEntry.Identifier = index.ToString("D");

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
        return itemType.GetMethod(nameof(ToString)).DeclaringType == typeof(object) ? itemType.Name : item.ToString();
    }

    private static string GetEntryDescription(object item)
    {
        var itemType = item.GetType();

        // Value type do not have names
        if (EntryConvert.ValueOrStringType(itemType))
        {
            return null;
        }

        // Check for display name declaration
        var description = itemType.GetDescription();
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        return description;
    }

    /// <summary>
    /// Generate a number of keys
    /// </summary>
    public static IEnumerable<string> GenerateKeys(int count)
        => count == 0 ? [] : Enumerable.Range(0, count).Select(i => i.ToString("D"));
}
