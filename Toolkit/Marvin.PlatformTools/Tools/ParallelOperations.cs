using System;
using System.Collections.Generic;
using System.Threading;
using Marvin.Modules;
using Marvin.Testing;
using Marvin.Threading;

namespace Marvin.Tools
{
    /// <summary>
    /// Describes an impossible exception.
    /// </summary>
    [OpenCoverIgnore]
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
    public class ParallelOperations : IParallelObservation
    {
        private readonly List<IParallelObserver> _observers = new List<IParallelObserver>();
    
        /// <summary>
        /// Dependency to report errors to plugin
        /// </summary>
        public IModuleErrorReporting FailureReporting { get; set; }
        
        /// <inheritdoc />
        public void Register(IParallelObserver observer)
        {
            _observers.Add(observer);
        }

        /// <inheritdoc />
        public void Unregister(IParallelObserver observer)
        {
            _observers.Remove(observer);
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
            foreach (var observer in _observers)
            {
                observer.Scheduled(operation, userState);
            }
            
            ThreadPool.QueueUserWorkItem(state =>
            {
                foreach (var observer in _observers)
                {
                    observer.Executing(operation, state);
                }

                try
                {
                    operation((T)state);
                }
                catch (Exception ex)
                {
                    HandleException(ex, operation, criticalOperation);
                }

                foreach (var observer in _observers)
                {
                    observer.Completed(operation, state);
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
            foreach (var observer in _observers)
            {
                observer.Scheduled(operation, userState, delayMs, periodMs);
            }

            var id = ++_lastTimerId;
            var timer = new Timer(new NonStackingTimerCallback(state =>
            {
                foreach (var observer in _observers)
                {
                    observer.Executing(operation, state);
                }

                try
                {
                    operation((T)state);
                    if(periodMs <= 0)
                        StopExecution(id);
                }
                catch (Exception ex)
                {
                    HandleException(ex, operation, criticalOperation);
                }

                foreach (var observer in _observers)
                {
                    observer.Completed(operation, state);
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

        /// <summary>
        /// Handle exception that occured in execution and would have killed the application
        /// </summary>
        private void HandleException(Exception ex, Delegate operation, bool criticalOperation)
        {
            var target = operation.Target ?? this;

            if (criticalOperation)
            {
                FailureReporting.ReportFailure(target, ex);
            }
            else
            {
                FailureReporting.ReportWarning(target, ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Dispose all timers
            foreach (var runningTimer in _runningTimers.Values)
            {
                runningTimer.Dispose();
            }
            _runningTimers.Clear();
        }
    }
}
