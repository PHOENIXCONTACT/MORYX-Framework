// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;

namespace Moryx.Notifications.Publisher
{
    internal class NotificationTypeList<T> : IReadOnlyList<T>
    {
        private readonly List<T> _underlyingList;
        private readonly ReaderWriterLockSlim _rwLockSlim;

        public NotificationTypeList(List<T> underlyingList, ReaderWriterLockSlim rwLockSlim)
        {
            _underlyingList = underlyingList;
            _rwLockSlim = rwLockSlim;
        }

        public IEnumerator<T> GetEnumerator() => new LockedEnumerator(_rwLockSlim, _underlyingList);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _underlyingList.Count;

        public T this[int index] => _underlyingList[index];

        private class LockedEnumerator : IEnumerator<T>
        {
            private readonly ReaderWriterLockSlim _rwLock;
            private List<T>.Enumerator _localEnumerator;

            public T Current => _localEnumerator.Current;

            object IEnumerator.Current => Current;

            public LockedEnumerator(ReaderWriterLockSlim rwRwLock, List<T> list)
            {
                _rwLock = rwRwLock;
                _rwLock.EnterReadLock();

                _localEnumerator = list.GetEnumerator();
            }

            public void Dispose()
            {
                _localEnumerator.Dispose();
                _rwLock.ExitReadLock();
            }

            public bool MoveNext() =>
                _localEnumerator.MoveNext();

            public void Reset() =>
                ((IEnumerator)_localEnumerator).Reset();
        }
    }
}
