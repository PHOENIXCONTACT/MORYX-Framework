// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Marvin.Threading;

namespace Marvin.TestTools.UnitTest
{
    internal class TimerInfo
    {
        public int TimerId { get; set; }
        public Timer Timer { get; set; }
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// Replacement for parallel operations to achive synchronus behaviour in unit tests.
    /// </summary>
    public class NotSoParallelOps : IParallelOperations
    {
        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel(Action operation)
        {
            ExecuteParallel(operation, false);
        }

        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel(Action operation, bool criticalOperation)
        {
            ExecuteParallel(state => operation(), new{}, criticalOperation);
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
            operation(userState);
        }

        /// <summary>
        /// Try until condition is true and then execute
        /// </summary>
        public void TryExecute(Action operation, int maxTries, int waitTimeMs)
        {
        }

        /// <summary>
        /// Try until condition is true and then execute
        /// </summary>
        public void TryExecute(Action operation, int maxTries, int waitTimeMs, bool criticalOperation)
        {
        }

        /// <summary>
        /// Try until condition is true and then execute
        /// </summary>
        public void TryExecute<T>(Action<T> operation, int maxTries, int waitTimeMs, T operationState) where T : class
        {
        }

        /// <summary>
        /// Try until condition is true and then execute
        /// </summary>
        public void TryExecute<T>(Action<T> operation, int maxTries, int waitTimeMs, T operationState, bool criticalOperation) where T : class
        {
        }

        private int _lastTimerId;
        private readonly CountdownEvent _countdown = new CountdownEvent(1);
        private readonly List<TimerInfo> _timers = new List<TimerInfo>();

        /// <inheritdoc />
        public int ScheduleExecution(Action operation, int delayMs, int periodMs)
        {
            return ScheduleExecution(state => operation(), new object(), delayMs, periodMs, false);
        }

        /// <inheritdoc />
        public int ScheduleExecution(Action operation, int delayMs, int periodMs, bool criticalOperation)
        {
            return ScheduleExecution(state => operation(), new object(), delayMs, periodMs, criticalOperation);
        }

        /// <inheritdoc />
        public int ScheduleExecution<T>(Action<T> operation, T userState, int delayMs, int periodMs) where T : class
        {
            return ScheduleExecution(operation, userState, delayMs, periodMs, false);
        }

        /// <inheritdoc />
        public int ScheduleExecution<T>(Action<T> operation, T userState, int delayMs, int periodMs, bool criticalOperation) where T : class
        {
            _countdown.AddCount();

            var id = _lastTimerId++;
            var timerInfo = new TimerInfo
            {
                TimerId = id
            };

            var timer = new Timer(new NonStackingTimerCallback(state =>
            {
                try
                {
                    operation(userState);
                }
                catch (Exception e)
                {
                    timerInfo.Exception = e;
                }
                finally
                {
                    if (periodMs <= 0)
                    {
                        StopExecution(id);
                    }
                }

            }), userState, delayMs, periodMs);

            timerInfo.Timer = timer;

            lock (_timers)
            {
                _timers.Add(timerInfo);
                return id;
            }
        }

        /// <inheritdoc />
        public void StopExecution(int timerId)
        {
            lock (_timers)
            {
                var timer = _timers.FirstOrDefault(t => t.TimerId == timerId);
                if (timer != null)
                {
                    _countdown.Signal();
                    timer.Timer.Dispose();
                }
            }
        }

        /// <inheritdoc />
        public EventHandler DecoupleListener(EventHandler<EventArgs> target)
        {
            return (o, e) => target(o, e);
        }

        /// <summary>
        /// Instead of decoupling it simply returns the same listener
        /// </summary>
        public EventHandler<TEventArgs> DecoupleListener<TEventArgs>(EventHandler<TEventArgs> target)
        {
            return target;
        }

        /// <inheritdoc />
        public EventHandler RemoveListener(EventHandler<EventArgs> target)
        {
            return (o, e) => target(o, e);
        }

        /// <summary>
        /// Instead of decoupling it simply returns the same listener
        /// </summary>
        public EventHandler<TEventArgs> RemoveListener<TEventArgs>(EventHandler<TEventArgs> target)
        {
            return target;
        }

        /// <summary>
        /// Waits until all scheduled executions has been finished. Note that all execution routines has to be called already.
        /// </summary>
        /// <returns>Returns true if all timers has been finished or false if not all timers has been finished when timeout is over</returns>
        public bool WaitForScheduledExecution(int timeoutMs)
        {
            lock (this)
            {
                _countdown.Signal();
                return _countdown.Wait(timeoutMs);
            }
        }

        /// <summary>
        /// Returns all exceptions that have been occurred within a scheduled execution operation
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Exception> ScheduledExecutionExceptions()
        {
            lock (this)
            {
                return _timers.Where(t => t.Exception != null).Select(t => t.Exception);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Dispose all timers
            foreach (var runningTimer in _timers)
            {
                runningTimer.Timer.Dispose();
            }

            lock (this)
            {
                if (_countdown.CurrentCount > 1)
                {
                    _countdown.Signal(_countdown.CurrentCount - 1);
                }
            }

            _timers.Clear();
        }
    }
}
