// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
    private IReadOnlyList<string> _newOrder;

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
        return _currentArray.GetValue(int.Parse(key));
    }

    public void Added(Entry entry, object addedValue)
    {
        _array.SetValue(addedValue, _index++);
    }

    public void Updated(Entry entry, object updatedValue)
    {
        _array.SetValue(updatedValue, _index++);
    }

    public void Removed(string key)
    {
    }

    /// <inheritdoc />
    public void Reorder(IReadOnlyList<string> newKeyOrder)
    {
        // Check if order actually changed
        var currentKeys = CollectionStrategyTools.GenerateKeys(_array.Length).ToList();
        if (currentKeys.SequenceEqual(newKeyOrder))
            return;

        // Rebuild array in new order
        var reordered = newKeyOrder.Select(key => _array.GetValue(int.Parse(key))).ToArray();
        for (var i = 0; i < reordered.Length; i++)
        {
            _array.SetValue(reordered[i], i);
        }
    }

    public void Flush()
    {
    }
}