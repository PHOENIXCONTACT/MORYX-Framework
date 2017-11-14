using System;

namespace Marvin.Collections
{
    /// <summary>
    /// Special queue for objects that need to be dequeued with a configured minimum delay.
    /// </summary>
    public interface IDelayQueue<T> 
        where T : class
    {
        /// <summary>
        /// Start the queue and start sendings objects with the given delay
        /// </summary>
        void Start(int queueDelay);

        /// <summary>
        /// Stop the queue and discard all pending objects
        /// </summary>
        void Stop();

        /// <summary>
        /// Adds an object to the end of the Queue.
        /// </summary>
        /// <param name="obj">The object to add to the queue.</param>
        void Enqueue(T obj);

        /// <summary>
        /// Removes and returns the object at the beginning of the Queue.
        /// </summary>
        event EventHandler<T> Dequeued;
    }
}