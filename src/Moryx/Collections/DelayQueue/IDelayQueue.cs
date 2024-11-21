// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Collections
{
    /// <summary>
    /// Special queue for objects that need to be dequeued with a configured minimum delay.
    /// </summary>
    public interface IDelayQueue<T>
        where T : class
    {
        /// <summary>
        /// Start the queue and start sendings items with the given delay
        /// </summary>
        void Start(int queueDelay);

        /// <summary>
        /// Stop the queue and discard all pending items
        /// </summary>
        void Stop();

        /// <summary>
        /// Adds an item to the end of the queue.
        /// </summary>
        /// <param name="obj">The item to add to the queue.</param>
        void Enqueue(T obj);

        /// <summary>
        /// Removes and returns the item at the beginning of the Queue.
        /// </summary>
        event EventHandler<T> Dequeued;
    }
}
