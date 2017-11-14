using System;
using System.Collections;
using System.Diagnostics;
using Marvin.StateMachines;
using Marvin.Threading;

namespace Marvin.Collections
{
    /// <summary>
    /// Special queue for messages that need to be transmitted with a configured minimum delay.
    /// </summary>
    public class DelayQueue<T> : IDelayQueue<T>, IDelayQueueContext
        where T : class
    {
        /// <summary>
        /// Parallel operations to delay message transmission
        /// </summary>
        private readonly IParallelOperations _parallelOperations;

        /// <summary>
        /// Internal state machine
        /// </summary>
        private QueueStateBase _state;

        /// <summary>
        /// Stopwatch to precisly measure time since last message
        /// </summary>
        private readonly Stopwatch _stopwatch;

        /// <summary>
        /// Explicit implementation to use in state machine
        /// </summary>
        Stopwatch IDelayQueueContext.Stopwatch => _stopwatch;

        /// <summary>
        /// Stopwatch to precisly measure time since last message
        /// </summary>
        private int _queueDelay;

        /// <summary>
        /// Explitic implementation to use in state machine
        /// </summary>
        int IDelayQueueContext.QueueDelay => _queueDelay;

        /// <summary>
        /// Used in state machine to manage pending messages of this queue
        /// </summary>
        Queue IDelayQueueContext.PendingMessages { get; set; }

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
            _state = (QueueStateBase) state;
        }

        /// 
        public void Start(int queueDelay)
        {
            _queueDelay = queueDelay;

            // ReSharper disable once InconsistentlySynchronizedField
            // Delay queue will be started
            _state.Start();
        }

        ///
        public void Stop()
        {
            lock (this)
                _state.Stop();
        }

        ///
        public void Enqueue(T obj)
        {
            lock (this)
                _state.Enqueue(obj);
        }

        /// <summary>
        /// Method called from the state machine to dequeue the object
        /// </summary>
        void IDelayQueueContext.ExecuteDequeue(object obj)
        {
            ExecuteDequeue((T) obj);
        }

        private void ExecuteDequeue(T obj)
        {
            lock (this)
            {
                var handler = Dequeued;
                handler?.Invoke(this, obj);

                _state.MessageSent(obj);
            }
        }

        /// <summary>
        /// Method called from the state machine to dequeue the object in a new thread
        /// </summary>
        void IDelayQueueContext.ExecuteDequeueParallel(object obj)
        {
            ExecuteDequeueParallel((T) obj);
        }

        private void ExecuteDequeueParallel(T obj)
        {
            _parallelOperations.ExecuteParallel(ExecuteDequeue, obj);
        }

        /// <summary>
        /// Method called from the state machine to delay the dequeue of the object
        /// </summary>
        void IDelayQueueContext.DequeueDelayed(object next, int queueDelay)
        {
            DequeueDelayed((T) next, queueDelay);
        }

        private void DequeueDelayed(T obj, int delay)
        {
            _parallelOperations.ScheduleExecution(DelayedExecute, obj, delay, -1);
        }

        /// <summary>
        /// Callback for <see cref="IParallelOperations"/> that locks access to
        /// the send method.
        /// </summary>
        private void DelayedExecute(T obj)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            // Will be only called by other thread
            _state.DelayedDequeue(obj);
        }

        ///
        public event EventHandler<T> Dequeued;
    }
}