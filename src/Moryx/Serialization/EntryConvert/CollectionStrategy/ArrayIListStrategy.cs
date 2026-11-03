// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Reflection;

namespace Moryx.Serialization;

/// <inheritdoc/>
public class ArrayIListStrategy : ICollectionStrategy
{
    private readonly IList _list;
    private readonly IList _toDelete = new List<object>();
    private readonly ICustomSerialization _customSerialization;
    private readonly ICustomAttributeProvider _property;
    private readonly object _instance;
    private readonly Dictionary<string, object> _addedItems = new();
    private IReadOnlyList<string> _newOrder;

    /// <inheritdoc/>
    public ArrayIListStrategy(IList list, ICustomSerialization customSerialization,
        ICustomAttributeProvider attributeProvider, object instance)
    {
        _list = list;
        _customSerialization = customSerialization;
        _property = attributeProvider;
        _instance = instance;
    }

    /// <inheritdoc/>
    public IEnumerable<Entry> Serialize()
    {
        var entries = new List<Entry>();
        for (int index = 0; index < _list.Count; index++)
        {
            var entry = CollectionStrategyTools.CreateSub(_list[index], index, _customSerialization);
            entries.Add(entry);
        }
        return entries;
    }

    /// <inheritdoc/>
    public IEnumerable<string> Keys()
    {
        return CollectionStrategyTools.GenerateKeys(_list.Count);
    }

    /// <inheritdoc/>
    public object ElementAt(string key)
    {
        return _list[int.Parse(key)];
    }

    /// <inheritdoc/>
    public void Added(Entry entry, object addedValue)
    {
        _addedItems[entry.Identifier] = addedValue;
    }

    /// <inheritdoc/>
    public void Updated(Entry entry, object updatedValue)
    {
        _list[int.Parse(entry.Identifier)] = updatedValue;
    }

    /// <inheritdoc/>
    public void Removed(string key)
    {
        _toDelete.Add(_list[int.Parse(key)]);
    }

    /// <inheritdoc />
    public void Reorder(IReadOnlyList<string> newKeyOrder)
    {
        if (CollectionStrategyTools.NeedsReordering(newKeyOrder, _list, _toDelete, _addedItems))
        {
            _newOrder = newKeyOrder;
        }
    }

    /// <inheritdoc/>
    public void Flush()
    {
        if (_property is PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;
            var elementType = type.GenericTypeArguments[0];

            if (_newOrder != null)
            {
                var itemsByKey = CollectionStrategyTools.BuildItemLookup(_list, _toDelete, _addedItems);

                // Rebuild array in the desired order
                var list = Array.CreateInstance(elementType, _newOrder.Count);
                var index = 0;
                foreach (var key in _newOrder)
                {
                    if (itemsByKey.TryGetValue(key, out var item))
                    {
                        list.SetValue(item, index++);
                    }
                }
                propertyInfo.SetValue(_instance, list);
            }
            else
            {
                // No reordering: filter deleted, append added
                var list = Array.CreateInstance(elementType, _list.Count - _toDelete.Count + _addedItems.Count);
                var index = 0;
                foreach (var e in _list)
                {
                    if (!_toDelete.Contains(e))
                    {
                        list.SetValue(e, index++);
                    }
                }
                foreach (var (_, value) in _addedItems)
                {
                    list.SetValue(value, index++);
                }
                propertyInfo.SetValue(_instance, list);
            }
        }
    }
}
