// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moryx.Logging;

namespace Moryx.Threading
{
    /// <summary>
    /// Describes an impossible exception.
    /// </summary>
    public class ImpossibleException : Exception
    {
        /// <summary>
        /// Constructor for an impossible exception.
        /// </summary>
        public ImpossibleException()
        {
        }
        /// <summary>
        /// Constructor for an impossible exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ImpossibleException(string message) : base(message)
        {
        }
        /// <summary>
        /// Constructor for an impossible exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">Inner exception if existing.</param>
        public ImpossibleException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    /// <summary>
    /// Should be moved to toolkit
    /// </summary>
    public class ParallelOperations : IParallelOperations
    {
        /// <summary>
        /// All active event decouples
        /// </summary>
        private readonly IDictionary<Delegate, object> _eventDecouplers = new Dictionary<Delegate, object>();

        /// <summary>
        /// Dependency to report errors to plugin
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Create parallel operations in service collection
        /// </summary>
        public ParallelOperations(ILogger<ParallelOperations> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Create parallel operations within module
        /// </summary>
        /// <param name="logger"></param>
        public ParallelOperations(IModuleLogger logger)
        {
            Logger = logger;
        }

        #region Execute Parallel
        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel(Action operation)
        {
            ExecuteParallel(state => operation(), new object(), false);
        }

        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel(Action operation, bool criticalOperation)
        {
            ExecuteParallel(state => operation(), new object(), criticalOperation);
        }

        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel<T>(Action<T> operation, T userState) where T : class
        {
            ExecuteParallel(operation, userState, false);
        }

        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel<T>(Action<T> operation, T userState, bool criticalOperation) where T : class
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    operation((T)state);
                }
                catch (Exception ex)
                {
                    HandleException(ex, operation, criticalOperation);
                }
            }, userState);
        }
        #endregion

        #region Execute Periodically
        private int _lastTimerId;
        private readonly Dictionary<int, Timer> _runningTimers = new Dictionary<int, Timer>();

        /// <summary>
        /// Execute non-critical operation periodically but non-stacking
        /// </summary>
        public int ScheduleExecution(Action operation, int delayMs, int periodMs)
        {
            return ScheduleExecution(state => operation(), new object(), delayMs, periodMs, false);
        }

        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        public int ScheduleExecution(Action operation, int delayMs, int periodMs, bool criticalOperation)
        {
            return ScheduleExecution(state => operation(), new object(), delayMs, periodMs, criticalOperation);
        }

        /// <summary>
        /// Execute non-critical operation periodically but non-stacking
        /// </summary>
        public int ScheduleExecution<T>(Action<T> operation, T userState, int delayMs, int periodMs) where T : class
        {
            return ScheduleExecution(operation, userState, delayMs, periodMs, false);
        }

        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        public int ScheduleExecution<T>(Action<T> operation, T userState, int delayMs, int periodMs, bool criticalOperation) where T : class
        {
            var id = ++_lastTimerId;
            var timer = new Timer(new NonStackingTimerCallback(state =>
            {
                try
                {
                    operation((T)state);
                    if (periodMs <= 0)
                        StopExecution(id);
                }
                catch (Exception ex)
                {
                    HandleException(ex, operation, criticalOperation);
                }
            }), userState, delayMs, periodMs);

            lock (_runningTimers)
            {
                _runningTimers[id] = timer;
                return id;
            }
        }

        /// <summary>
        /// Stop the execution of the timer with this id
        /// </summary>
        /// <param name="timerId"></param>
        public void StopExecution(int timerId)
        {
            lock (_runningTimers)
            {
                if (!_runningTimers.ContainsKey(timerId))
                    return;

                var timer = _runningTimers[timerId];
                timer.Dispose();
                _runningTimers.Remove(timerId);
            }
        }

        #endregion

        #region Event Decoupling

        /// <inheritdoc />
        public EventHandler DecoupleListener(EventHandler<EventArgs> target)
        {
            var decoupler = CreateDecoupler(target);
            return decoupler.EventListener;
        }

        /// <inheritdoc />
        public EventHandler<TEventArgs> DecoupleListener<TEventArgs>(EventHandler<TEventArgs> target)
        {
            var decoupler = CreateDecoupler(target);
            return decoupler.EventListener;
        }

        /// <summary>
        /// Create a typed event decoupler for the given listener delegate
        /// </summary>
        private EventDecoupler<TEventArgs> CreateDecoupler<TEventArgs>(EventHandler<TEventArgs> target)
        {
            // Extract logger from target
            var logger = ExtractLoggerFromDelegate(target);
            var decoupler = new EventDecoupler<TEventArgs>(target, this, logger);

            lock (_eventDecouplers)
                _eventDecouplers.Add(target, decoupler);

            return decoupler;
        }

        /// <inheritdoc />
        public EventHandler RemoveListener(EventHandler<EventArgs> target)
        {
            var decoupler = RemoveDecoupler(target);
            return decoupler.EventListener;
        }

        /// <inheritdoc />
        public EventHandler<TEventArgs> RemoveListener<TEventArgs>(EventHandler<TEventArgs> target)
        {
            var decoupler = RemoveDecoupler(target);
            return decoupler.EventListener;
        }

        /// <summary>
        /// Remove the decoupler from the collection and dispose it
        /// </summary>
        private EventDecoupler<TEventArgs> RemoveDecoupler<TEventArgs>(EventHandler<TEventArgs> target)
        {
            EventDecoupler<TEventArgs> decoupler;
            lock (_eventDecouplers)
            {
                if (!_eventDecouplers.ContainsKey(target))
                    throw new InvalidOperationException("Can not remove a previously unregistered listener!");

                decoupler = (EventDecoupler<TEventArgs>)_eventDecouplers[target];
                _eventDecouplers.Remove(target);
            }
            return decoupler;
        }

        #endregion

        /// <summary>
        /// Handle exception that occurred in execution and would have killed the application
        /// </summary>
        private void HandleException(Exception ex, Delegate operation, bool criticalOperation)
        {
            var logger = ExtractLoggerFromDelegate(operation);
            logger.Log(criticalOperation ? LogLevel.Critical : LogLevel.Error, ex,
                "Exception during ParallelOperations of {0}.{1}!", operation.Method.DeclaringType?.Name ?? "Unknown", operation.Method.Name);
        }

        /// <summary>
        /// Try to extract a logger from the operation
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private ILogger ExtractLoggerFromDelegate(Delegate operation)
        {
            var target = operation?.Target ?? this;
            var logger = (target as Logging.ILoggingComponent)?.Logger ?? Logger;
            return logger;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Dispose all timers and event decouples
            foreach (var runningTimer in _runningTimers.Values)
            {
                runningTimer.Dispose();
            }
            // Dispose all
            _runningTimers.Clear();
        }

        /// <summary>
        /// The <see cref="EventDecoupler{TEventArgs}"/> provides an event listener that enqueues incoming calls
        /// and forwards them on a dedicated worker thread
        /// </summary>
        /// <typeparam name="TEventArgs">Arguments for the <see cref="EventHandler{TEventArgs}"/> delegate</typeparam>
        private class EventDecoupler<TEventArgs> : ParallelOperationsQueue<Tuple<object, TEventArgs>>
        {
            /// <summary>
            /// Create a new <see cref="EventDecoupler{TEventArgs}"/> to decouple a single listener from an event
            /// </summary>
            public EventDecoupler(EventHandler<TEventArgs> eventTarget, IParallelOperations parallelOperations, ILogger logger)
             : base(elem => eventTarget(elem.Item1, elem.Item2), parallelOperations, logger)
            {
            }

            public void EventListener(object sender, TEventArgs eventArgs)
            {
                Enqueue(new Tuple<object, TEventArgs>(sender, eventArgs));
            }
        }
    }
}
