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
    private readonly IList _toAdd = new List<object>();

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
        _toAdd.Add(addedValue);
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

    /// <inheritdoc/>
    public void Flush()
    {

        if (_property is PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;
            var elementType = type.GenericTypeArguments[0];
            var list = Array.CreateInstance(elementType, _list.Count - _toDelete.Count + _toAdd.Count);
            var index = 0;
            foreach (var e in _list)
            {
                if (!_toDelete.Contains(e))
                {
                    list.SetValue(e, index++);
                }
            }
            foreach (var e in _toAdd)
            {
                list.SetValue(e, index++);
            }

            propertyInfo.SetValue(_instance, list);
        }
    }
}