// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;

namespace Moryx.Serialization;

/// <summary>
/// Array implementation of <see cref="ICollectionStrategy"/>
/// </summary>
internal class ArrayStrategy : ICollectionStrategy
{
    private int _index;
    private readonly Array _array;
    private readonly Array _currentArray;
    private readonly ICustomSerialization _serialization;

    /// <summary>
    /// Maps entry identifiers to their array positions.
    /// Required because identifiers can be non-numeric (e.g. "CREATED-guid").
    /// </summary>
    private readonly Dictionary<string, int> _keyToIndex = new();

    public ArrayStrategy(Array array, Array currentArray, ICustomSerialization serialization)
    {
        _array = array;
        _currentArray = currentArray;
        _serialization = serialization;
    }

    public IEnumerable<Entry> Serialize()
    {
        var entries = new List<Entry>();
        for (var i = 0; i < _array.Length; i++)
        {
            var value = _array.GetValue(i);
            var entry = CollectionStrategyTools.CreateSub(value, i, _serialization);
            entries.Add(entry);
        }
        return entries;
    }

    public IEnumerable<string> Keys()
    {
        return CollectionStrategyTools.GenerateKeys(_currentArray.Length);
    }

    public object ElementAt(string key)
    {
        return _currentArray.GetValue(int.Parse(key, CultureInfo.InvariantCulture));
    }

    public void Added(Entry entry, object addedValue)
    {
        _keyToIndex[entry.Identifier] = _index;
        _array.SetValue(addedValue, _index++);
    }

    public void Updated(Entry entry, object updatedValue)
    {
        _keyToIndex[entry.Identifier] = _index;
        _array.SetValue(updatedValue, _index++);
    }

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
            .Select(key => _array.GetValue(_keyToIndex[key]))
            .ToArray();
        for (var i = 0; i < reordered.Length; i++)
        {
            _array.SetValue(reordered[i], i);
        }
    }

    public void Flush()
    {
    }
}
