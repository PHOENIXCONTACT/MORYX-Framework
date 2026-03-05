// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Moryx.Modules;

namespace Moryx.Runtime.Modules;

internal class ServerNotificationCollection : INotificationCollection
{
    private const int MaxCollectionSize = 2500;
    private readonly Collection<IModuleNotification> _internalList = [];
    private readonly Lock _lockObj = new();

    // ReSharper disable once InconsistentlySynchronizedField
    public int Count => _internalList.Count;

    public bool IsReadOnly => false;

    public IEnumerator<IModuleNotification> GetEnumerator()
    {
        List<IModuleNotification> copy;
        lock (_lockObj)
        {
            copy = _internalList.ToList();
        }

        return copy.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(IModuleNotification item)
    {
        IModuleNotification removedItem = null;
        lock (_lockObj)
        {
            _internalList.Add(item);
            if (_internalList.Count > MaxCollectionSize)
            {
                removedItem = _internalList.First();
                _internalList.Remove(removedItem);
            }
        }

        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));

        if (removedItem != null)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }
    }

    public void Clear()
    {
        lock (_lockObj)
        {
            _internalList.Clear();
        }

        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public bool Contains(IModuleNotification item)
    {
        bool contains;
        lock (_lockObj)
        {
            contains = _internalList.Contains(item);
        }

        return contains;
    }

    public void CopyTo(IModuleNotification[] array, int arrayIndex)
    {
        lock (_lockObj)
        {
            _internalList.CopyTo(array, arrayIndex);
        }
    }

    public bool Remove(IModuleNotification item)
    {
        bool removed;
        lock (_lockObj)
        {
            removed = _internalList.Remove(item);
        }

        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        return removed;
    }

    public event NotifyCollectionChangedEventHandler CollectionChanged;
}
