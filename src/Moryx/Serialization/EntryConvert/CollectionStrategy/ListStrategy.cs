// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;

namespace Moryx.Serialization;

/// <summary>
/// <see cref="IList"/> implementation of <see cref="ICollectionStrategy"/>
/// </summary>
internal class ListStrategy(IList list, ICustomSerialization customSerialization) : ICollectionStrategy
{
    private readonly IList _toDelete = new List<object>();

    /// <summary>
    /// Optional encoder for the items of the list
    /// </summary>
    internal Func<object, Entry> EncodeItem { get; init; }

    public IEnumerable<Entry> Serialize()
    {
        var entries = new List<Entry>();
        for (int index = 0; index < list.Count; index++)
        {
            var entry = CollectionStrategyTools.CreateSub(list[index], index, customSerialization, EncodeItem);
            entries.Add(entry);
        }
        return entries;
    }

    /// <inheritdoc/>
    public IEnumerable<string> Keys() => CollectionStrategyTools.GenerateKeys(list.Count);

    /// <inheritdoc/>
    public object ElementAt(string key) => list[int.Parse(key)];

    /// <inheritdoc/>
    public void Added(Entry entry, object addedValue) => list.Add(addedValue);

    /// <inheritdoc/>
    public void Updated(Entry entry, object updatedValue) => list[int.Parse(entry.Identifier)] = updatedValue;

    /// <inheritdoc/>
    public void Removed(string key) => _toDelete.Add(list[int.Parse(key)]);

    /// <inheritdoc/>
    public void Flush()
    {
        foreach (var missing in _toDelete)
        {
            list.Remove(missing);
        }
    }
}
