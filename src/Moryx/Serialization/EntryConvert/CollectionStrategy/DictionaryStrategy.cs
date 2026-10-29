// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;

namespace Moryx.Serialization;

/// <summary>
/// Dictionary implementation of <see cref="ICollectionStrategy"/>
/// </summary>
internal class DictionaryStrategy(IDictionary dictionary, ICustomSerialization serialization) : ICollectionStrategy
{
    /// <summary>
    /// Optional encoder for the values of the dictionary
    /// </summary>
    internal Func<object, Entry> EncodeItem { get; init; }

    /// <inheritdoc/>
    public IEnumerable<Entry> Serialize()
    {
        var entries = new List<Entry>();
        foreach (var key in dictionary.Keys)
        {
            var value = dictionary[key];
            var keyString = key.ToString();

            var subEntry = CollectionStrategyTools.EncodeItem(value, serialization, EncodeItem);
            subEntry.DisplayName = keyString;
            subEntry.Identifier = keyString;

            entries.Add(subEntry);
        }
        return entries;
    }

    /// <inheritdoc/>
    public IEnumerable<string> Keys() => dictionary.Keys.OfType<object>().Select(key => key.ToString()).ToArray();

    /// <inheritdoc/>
    public object ElementAt(string key)
    {
        var keyValue = EntryValue(key);
        return dictionary[keyValue];
    }

    /// <inheritdoc/>
    public void Added(Entry entry, object addedValue)
    {
        var keyValue = EntryValue(entry.DisplayName);
        dictionary.Add(keyValue, addedValue);
    }

    /// <inheritdoc/>
    public void Updated(Entry entry, object updatedValue)
    {
        Removed(entry.Identifier);
        Added(entry, updatedValue);
    }

    /// <inheritdoc/>
    public void Removed(string key)
    {
        var entryValue = EntryValue(key);
        dictionary.Remove(entryValue);
    }

    /// <inheritdoc/>
    public void Flush()
    {
    }

    /// <inheritdoc/>
    private object EntryValue(string entry)
    {
        var entryType = dictionary.GetType().GenericTypeArguments[0];
        var entryValue = EntryConvert.ToObject(entryType, entry, serialization.FormatProvider);
        return entryValue;
    }
}
