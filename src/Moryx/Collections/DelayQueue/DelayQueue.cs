// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Diagnostics;
using Moryx.StateMachines;
using Moryx.Threading;

namespace Moryx.Collections
{
    /// <summary>
    /// Special queue for items that need to be dequeued with a configured minimum delay.
    /// </summary>
    public class DelayQueue<T> : IDelayQueue<T>, IDelayQueueContext
        where T : class
    {
        /// <summary>
        /// Parallel operations to delay item dequeuing
        /// </summary>
        private readonly IParallelOperations _parallelOperations;

        /// <summary>
        /// Internal state machine
        /// </summary>
        private QueueStateBase _state;

        /// <summary>
        /// Stopwatch to precisly measure time since last dequeue
        /// </summary>
        private readonly Stopwatch _stopwatch;

        /// <summary>
        /// Lock object for this instance of the queue
        /// </summary>
        private readonly object _stateLock = new();

        /// <summary>
        /// Explicit implementation to use in state machine
        /// </summary>
        Stopwatch IDelayQueueContext.Stopwatch => _stopwatch;

        /// <summary>
        /// Stopwatch to precisly measure time since last dequeue
        /// </summary>
        private int _queueDelay;

        /// <summary>
        /// Explitic implementation to use in state machine
        /// </summary>
        int IDelayQueueContext.QueueDelay => _queueDelay;

        /// <summary>
        /// Used in state machine to manage pending items of this queue
        /// </summary>
        Queue IDelayQueueContext.PendingItems { get; } = new Queue();

        /// <summary>
        /// Create a new instance of the <see cref="DelayQueue{TMessage}"/>
        /// </summary>
        public DelayQueue(IParallelOperations parallelOperations)
        {
            _parallelOperations = parallelOperations;
            _stopwatch = new Stopwatch();

            StateMachine.Initialize<IDelayQueueContext>(this).With<QueueStateBase>();
        }

        void IStateContext.SetState(IState state)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            // All state changes will be do in locked block
            _state = (QueueStateBase)state;
        }

        /// <inheritdoc />
        public void Start(int queueDelay)
        {
            _queueDelay = queueDelay;

            // ReSharper disable once InconsistentlySynchronizedField
            // Delay queue will be started
            _state.Start();
        }

        /// <inheritdoc />
        public void Stop()
        {
            lock (_stateLock)
                _state.Stop();
        }

        /// <inheritdoc />
        public void Enqueue(T obj)
        {
            lock (_stateLock)
                _state.Enqueue(obj);
        }

        /// <summary>
        /// Method called from the state machine to dequeue the item
        /// </summary>
        void IDelayQueueContext.ExecuteDequeue(object item)
        {
            lock (_stateLock)
            {
                var handler = Dequeued;
                handler?.Invoke(this, (T)item);

                _state.MessageSent(item);
            }
        }

        /// <summary>
        /// Method called from the state machine to dequeue the item in a new thread
        /// </summary>
        void IDelayQueueContext.ExecuteDequeueParallel(object item)
        {
            _parallelOperations.ExecuteParallel(DelayedExecute, item);
        }

        /// <summary>
        /// Method called from the state machine to delay the dequeuing of the item
        /// </summary>
        void IDelayQueueContext.DequeueDelayed(object next, int queueDelay)
        {
            _parallelOperations.ScheduleExecution(DelayedExecute, next, queueDelay, -1);
        }

        /// <summary>
        /// Callback for <see cref="IParallelOperations"/> that locks access to
        /// the send method.
        /// </summary>
        private void DelayedExecute(object obj)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            // Will be only called by other thread
            _state.DelayedDequeue(obj);
        }

        /// <inheritdoc />
        public event EventHandler<T> Dequeued;
    }
}
