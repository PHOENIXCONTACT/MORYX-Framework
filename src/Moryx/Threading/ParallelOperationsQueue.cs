// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Moryx.Threading
{
    /// <summary>
    /// Task queue that utilizes parallel operations and to execute tasks
    /// on a different thread WITHOUT blocking a thread during inactivity.
    /// The queue preserves the order of execution and avoids parallel execution
    /// of two tasks
    /// </summary>
    public class ParallelOperationsQueue<TElement>
    {
        /// <summary>
        /// Thread counter of active
        /// </summary>
        private int _pendingElements;

        /// <summary>
        /// Queue of unprocessed events
        /// </summary>
        private readonly ConcurrentQueue<TElement> _eventQueue = new ConcurrentQueue<TElement>();

        /// <summary>
        /// Target callback for the event
        /// </summary>
        private Action<TElement> ElementExecution { get; }

        /// <summary>
        /// Reference to the managing <see cref="IParallelOperations"/> instance
        /// </summary>
        private IParallelOperations ParallelOperations { get; }

        /// <summary>
        /// Logger instance to log errors
        /// </summary>
        public ILogger ErrorLogger { get; }

        /// <summary>
        /// Create a new <see cref="Threading.ParallelOperations.EventDecoupler{TEventArgs}"/> to decouple a single listener from an event
        /// </summary>
        public ParallelOperationsQueue(Action<TElement> elementExecution, IParallelOperations parallelOperations, ILogger errorLogger)
        {
            ElementExecution = elementExecution;
            ParallelOperations = parallelOperations;
            ErrorLogger = errorLogger;
        }

        /// <summary>
        /// Target for the worker thread to process the event queue
        /// </summary>
        private void ProcessEventQueue()
        {
            do
            {
                _eventQueue.TryDequeue(out var nextElement);

                try
                {
                    ElementExecution.Invoke(nextElement);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(LogLevel.Error, ex, "Exception during queue element execution!");
                }
            } while (Interlocked.Decrement(ref _pendingElements) > 0);
        }

        /// <summary>
        /// Enqueue a new element that shall be processed in a different thread from the queue
        /// </summary>
        /// <param name="element"></param>
        public void Enqueue(TElement element)
        {
            _eventQueue.Enqueue(element);

            if (Interlocked.Increment(ref _pendingElements) == 1)
                ParallelOperations.ExecuteParallel(ProcessEventQueue);
        }
    }
}
