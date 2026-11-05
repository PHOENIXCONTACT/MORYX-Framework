// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Globalization;

namespace Moryx.Serialization;

/// <summary>
/// <see cref="IList"/> implementation of <see cref="ICollectionStrategy"/>
/// </summary>
internal class ListStrategy(IList list, ICustomSerialization customSerialization) : ICollectionStrategy
{
    private readonly IList _toDelete = new List<object>();
    private readonly Dictionary<string, object> _addedItems = new();
    private IReadOnlyList<string> _newOrder;

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
    public object ElementAt(string key) => list[int.Parse(key, CultureInfo.InvariantCulture)];

    /// <inheritdoc/>
    public void Added(Entry entry, object addedValue) => _addedItems[entry.Identifier] = addedValue;

    /// <inheritdoc/>
    public void Updated(Entry entry, object updatedValue) => list[int.Parse(entry.Identifier, CultureInfo.InvariantCulture)] = updatedValue;

    /// <inheritdoc/>
    public void Removed(string key) => _toDelete.Add(list[int.Parse(key, CultureInfo.InvariantCulture)]);

    /// <inheritdoc />
    public void Reorder(IReadOnlyList<string> newKeyOrder)
    {
        if (CollectionStrategyTools.NeedsReordering(newKeyOrder, list, _toDelete, _addedItems))
        {
            _newOrder = newKeyOrder;
        }
    }

    /// <inheritdoc/>
    public void Flush()
    {
        if (_newOrder != null)
        {
            var itemsByKey = CollectionStrategyTools.BuildItemLookup(list, _toDelete, _addedItems);

            list.Clear();
            foreach (var key in _newOrder)
            {
                if (itemsByKey.TryGetValue(key, out var item))
                {
                    list.Add(item);
                }
            }
        }
        else
        {
            // No reordering: apply deletions and append new items
            foreach (var missing in _toDelete)
            {
                list.Remove(missing);
            }
            foreach (var (_, value) in _addedItems)
            {
                list.Add(value);
            }
        }
    }
}
