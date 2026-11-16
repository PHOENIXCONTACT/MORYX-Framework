// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;

namespace Moryx.Serialization;

/// <summary>
/// <see cref="Array"/> implementation of <see cref="ICollectionStrategy"/>
/// </summary>
internal class ArrayStrategy(Array array, Array currentArray, ICustomSerialization serialization) : ICollectionStrategy
{
    private int _index;

    /// <summary>
    /// Maps entry identifiers to their array positions.
    /// Required because identifiers can be non-numeric (e.g. "CREATED-guid").
    /// </summary>
    private readonly Dictionary<string, int> _keyToIndex = new();

    /// <summary>
    /// Optional encoder for the items of the array
    /// </summary>
    internal Func<object, Entry> EncodeItem { get; init; }

    public IEnumerable<Entry> Serialize()
    {
        var entries = new List<Entry>();
        for (var i = 0; i < array.Length; i++)
        {
            var value = array.GetValue(i);
            var entry = CollectionStrategyTools.CreateSub(value, i, serialization, EncodeItem);
            entries.Add(entry);
        }
        return entries;
    }

    /// <inheritdoc/>
    public IEnumerable<string> Keys() => CollectionStrategyTools.GenerateKeys(currentArray.Length);

    /// <inheritdoc/>
    public object ElementAt(string key) => currentArray.GetValue(int.Parse(key, CultureInfo.InvariantCulture));

    /// <inheritdoc/>
    public void Added(Entry entry, object addedValue)
    {
        _keyToIndex[entry.Identifier] = _index;
        array.SetValue(addedValue, _index++);
    }

    /// <inheritdoc/>
    public void Updated(Entry entry, object updatedValue)
    {
        _keyToIndex[entry.Identifier] = _index;
        array.SetValue(updatedValue, _index++);
    }

    /// <inheritdoc/>
    public void Removed(string key)
    {
    }

    /// <inheritdoc />
    public void Reorder(IReadOnlyList<string> newKeyOrder)
    {
        // TODO: Remove in next major version once all consumers use unique CREATED identifiers
        // Skip reorder if keys aren't unique (e.g. multiple "CREATED" entries)
        if (newKeyOrder.Count != newKeyOrder.Distinct().Count())
        {
            return;
        }

        if (_keyToIndex.Keys.SequenceEqual(newKeyOrder))
        {
            return;
        }

        var reordered = newKeyOrder
            .Where(key => _keyToIndex.ContainsKey(key))
            .Select(key => array.GetValue(_keyToIndex[key]))
            .ToArray();
        for (var i = 0; i < reordered.Length; i++)
        {
            array.SetValue(reordered[i], i);
        }
    }

    /// <inheritdoc/>
    public void Flush()
    {
    }
}
