// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Reflection;

namespace Moryx.Serialization;

// TODO: Make internal in MORYX 12
/// <summary>
/// <see cref="IList"/> implementation of <see cref="ICollectionStrategy"/>
/// </summary>
public class ArrayIListStrategy(IList list, ICustomSerialization customSerialization,
    ICustomAttributeProvider attributeProvider, object instance) : ICollectionStrategy
{
    private readonly IList _toDelete = new List<object>();
    private readonly IList _toAdd = new List<object>();

    /// <summary>
    /// Optional encoder for the items of the array
    /// </summary>
    internal Func<object, Entry> EncodeItem { get; init; }

    /// <inheritdoc/>
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
    public void Added(Entry entry, object addedValue) => _toAdd.Add(addedValue);

    /// <inheritdoc/>
    public void Updated(Entry entry, object updatedValue) => list[int.Parse(entry.Identifier)] = updatedValue;

    /// <inheritdoc/>
    public void Removed(string key) => _toDelete.Add(list[int.Parse(key)]);

    /// <inheritdoc/>
    public void Flush()
    {

        if (attributeProvider is PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;
            var elementType = type.GenericTypeArguments[0];
            var array = Array.CreateInstance(elementType, list.Count - _toDelete.Count + _toAdd.Count);
            var index = 0;
            foreach (var e in list)
            {
                if (!_toDelete.Contains(e))
                {
                    array.SetValue(e, index++);
                }
            }
            foreach (var e in _toAdd)
            {
                array.SetValue(e, index++);
            }

            propertyInfo.SetValue(instance, array);
        }
    }
}
