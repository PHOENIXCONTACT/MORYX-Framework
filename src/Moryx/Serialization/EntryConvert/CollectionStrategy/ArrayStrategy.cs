// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization;

/// <summary>
/// <see cref="Array"/> implementation of <see cref="ICollectionStrategy"/>
/// </summary>
internal class ArrayStrategy(Array array, Array currentArray, ICustomSerialization serialization) : ICollectionStrategy
{
    private int _index;

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
    public object ElementAt(string key) => currentArray.GetValue(int.Parse(key));

    /// <inheritdoc/>
    public void Added(Entry entry, object addedValue) => array.SetValue(addedValue, _index++);

    /// <inheritdoc/>
    public void Updated(Entry entry, object updatedValue) => array.SetValue(updatedValue, _index++);

    /// <inheritdoc/>
    public void Removed(string key)
    {
    }

    /// <inheritdoc/>
    public void Flush()
    {
    }
}
