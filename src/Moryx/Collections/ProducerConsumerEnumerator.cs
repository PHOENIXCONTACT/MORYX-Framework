// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;

namespace Moryx.Collections
{
    /// <summary>
    /// Represents a ReaderWriterLockSlim synced enumerator to iterate over a producer-consumer collection
    /// </summary>
    public class ProducerConsumerEnumerator<T> : IEnumerator<T>
    {
        private readonly ReaderWriterLockSlim _rwLock;
        private readonly IEnumerator<T> _innerEnumerator;

        /// <inheritdoc />
        public T Current => _innerEnumerator.Current;

        object IEnumerator.Current => Current;

        /// <summary>
        /// Constructor for an ReaderWriterLockSlim based enumerator
        /// </summary>
        /// <param name="rwLock">ReaderWriterLockSlim that can be used for synchronized access</param>
        /// <param name="enumerable">Enumerable to enumerate</param>
        public ProducerConsumerEnumerator(ReaderWriterLockSlim rwLock, IEnumerable<T> enumerable)
        {
            _rwLock = rwLock;
            _rwLock.EnterReadLock();

            _innerEnumerator = enumerable.GetEnumerator();
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
            _rwLock.ExitReadLock();
        }
    }
}
