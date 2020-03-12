// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Marvin.Collections
{
    /// <summary>
    /// Represents a synced enumerator to iterate over a generic collection
    /// </summary>
    public class MonitorEnumerator<T> : IEnumerator<T>
    {
        private readonly object _syncRoot;
        private readonly IEnumerator<T> _innerEnumerator;

        /// <inheritdoc />
        public T Current => _innerEnumerator.Current;

        object IEnumerator.Current => Current;

        /// <summary>
        /// Constructor for an monitor based enumerator
        /// </summary>
        /// <param name="syncRoot">An object that can be used for synchronized access</param>
        /// <param name="enumerable">Enumerable to enumerate</param>
        public MonitorEnumerator(object syncRoot, IEnumerable<T> enumerable)
        {
            _syncRoot = syncRoot;
            _innerEnumerator = enumerable.GetEnumerator();

            Monitor.Enter(_syncRoot);
        }

        /// <inheritdoc />
        public virtual bool MoveNext() =>
            _innerEnumerator.MoveNext();

        /// <inheritdoc />
        public virtual void Reset() =>
            _innerEnumerator.Reset();

        /// <inheritdoc />
        public virtual void Dispose()
        {
            _innerEnumerator.Dispose();
            Monitor.Exit(_syncRoot);
        }
    }
}
