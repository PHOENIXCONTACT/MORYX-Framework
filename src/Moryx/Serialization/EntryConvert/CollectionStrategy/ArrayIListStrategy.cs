// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Globalization;
using System.Reflection;

namespace Moryx.Serialization;

// TODO: Make internal in next major MORYX release
/// <summary>
/// <see cref="IList"/> implementation of <see cref="ICollectionStrategy"/>
/// </summary>
public class ArrayIListStrategy(IList list, ICustomSerialization customSerialization,
    ICustomAttributeProvider attributeProvider, object instance) : ICollectionStrategy
{
    private readonly IList _toDelete = new List<object>();
    private readonly Dictionary<string, object> _addedItems = [];
    private IReadOnlyList<string> _newOrder;

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
    public object ElementAt(string key) => list[int.Parse(key, CultureInfo.InvariantCulture)];

    /// <inheritdoc/>
    public void Added(Entry entry, object addedValue)
    {
        _addedItems[entry.Identifier] = addedValue;
    }

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
        if (attributeProvider is PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;
            var elementType = type.GenericTypeArguments[0];

            if (_newOrder != null)
            {
                var itemsByKey = CollectionStrategyTools.BuildItemLookup(list, _toDelete, _addedItems);

                // Rebuild array in the desired order
                var array = Array.CreateInstance(elementType, _newOrder.Count);
                var index = 0;
                foreach (var key in _newOrder)
                {
                    if (itemsByKey.TryGetValue(key, out var item))
                    {
                        array.SetValue(item, index++);
                    }
                }
                propertyInfo.SetValue(instance, list);
            }
            else
            {
                // No reordering: filter deleted, append added
                var array = Array.CreateInstance(elementType, list.Count - _toDelete.Count + _addedItems.Count);
                var index = 0;
                foreach (var e in list)
                {
                    if (!_toDelete.Contains(e))
                    {
                        array.SetValue(e, index++);
                    }
                }
                foreach (var (_, value) in _addedItems)
                {
                    array.SetValue(value, index++);
                }
                propertyInfo.SetValue(instance, list);
            }
        }
    }
}
