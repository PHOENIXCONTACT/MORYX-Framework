using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Marvin.Modules;

namespace Marvin.Runtime.Modules
{
    internal class ServerNotificationCollection : INotificationCollection
    {
        private readonly List<IModuleNotification> _internalList = new List<IModuleNotification>();
        private readonly object _lockObj = new object();

        // ReSharper disable once InconsistentlySynchronizedField
        public int Count => _internalList.Count;

        public bool IsReadOnly => false;

        public IEnumerator<IModuleNotification> GetEnumerator()
        {
            List<IModuleNotification> copy;
            lock (_lockObj)
                copy = _internalList.ToList();

            return copy.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(IModuleNotification item)
        {
            lock (_lockObj)
                _internalList.Add(item);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            lock (_lockObj)
                _internalList.Clear();

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(IModuleNotification item)
        {
            bool contains;
            lock (_lockObj)
                contains = _internalList.Contains(item);

            return contains;
        }

        public void CopyTo(IModuleNotification[] array, int arrayIndex)
        {
            lock (_lockObj)
                _internalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(IModuleNotification item)
        {
            bool removed;
            lock (_lockObj)
                removed = _internalList.Remove(item);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return removed;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}