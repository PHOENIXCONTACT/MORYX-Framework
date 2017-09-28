using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Threading;

namespace Marvin.Runtime.Base
{
    internal class ExecutionCollection
    {
        public Action<object>[] Operations { get; set; }
        public object[] UserStates { get; set; }
        public bool Critical { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
    }

    internal class RetriedExecution
    {
        public Action<object> Operation { get; set; }
        public object OperationState { get; set; }
        public int MaxTries { get; set; }
        public int Tries { get; set; }
        public int TimerId { get; set; }
    }

    public class ImpossibleException : Exception
    {
        public ImpossibleException()
        {
        }
        public ImpossibleException(string message) : base(message)
        {
        }
        public ImpossibleException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    /// <summary>
    /// Should be moved to toolkit
    /// </summary>
    [Plugin(LifeCycle.Singleton, typeof(IParallelOperations), DontIntercept = true)]
    internal class InternalThreadFactory : IParallelOperations
    {
        // Set by castle to report errors to plugin
        public IModuleErrorReporting FailureReporting { get; set; }
        public IModuleLogger Logger { get; set; }

        #region Execute Parallel
        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel(Action operation)
        {
            ExecuteParallel(state => operation(), null, false);
        }

        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel(Action operation, bool criticalOperation)
        {
            ExecuteParallel(state => operation(), null, criticalOperation);
        }

        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel(Action<object> operation, object userState)
        {
            ExecuteParallel(operation, userState, false);
        }

        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel(Action<object> operation, object userState, bool criticalOperation)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    operation(userState);
                }
                catch (Exception ex)
                {
                    HandleException(ex, operation, criticalOperation);
                }
            }, userState);
        }
        #endregion

        #region Execute collection parallel
        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel(IEnumerable<Action> operations, int maxParallel)
        {
            var opBuffer = operations.Select(o => new Action<object>(state => o())).ToArray();
            ExecuteCollectionParallel(opBuffer, new object[opBuffer.Length], false, maxParallel);
        }

        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel(IEnumerable<Action> operations, int maxParallel, bool criticalOperation)
        {
            var opBuffer = operations.Select(o => new Action<object>(state => o())).ToArray();
            ExecuteCollectionParallel(opBuffer, new object[opBuffer.Length], criticalOperation, maxParallel);
        }

        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel(IEnumerable<Action<object>> operations, int maxParallel, IEnumerable<object> userStates)
        {
            ExecuteCollectionParallel(operations.ToArray(), userStates.ToArray(), false, maxParallel);
        }

        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        public void ExecuteParallel(IEnumerable<Action<object>> operations, int maxParallel, IEnumerable<object> userStates, bool criticalOperation)
        {
            ExecuteCollectionParallel(operations.ToArray(), userStates.ToArray(), criticalOperation, maxParallel);
        }

        private void ExecuteCollectionParallel(Action<object>[] operations, object[] states, bool criticalOperation, int maxParallel)
        {
            if (operations.Length != states.Length)
            {
                Logger.LogEntry(LogLevel.Warning, "Number of elements in operations does not match number of user states!");
                return;
            }

            var execution = new ExecutionCollection
            {
                Operations = operations,
                UserStates = states,
                Critical = criticalOperation,
                Length = operations.Length
            };
            for (var threadNumber = 0; threadNumber < maxParallel; threadNumber++)
            {
                ThreadPool.QueueUserWorkItem(ExecuteCollection, execution);
            }
        }

        private void ExecuteCollection(object collectionObj)
        {
            var collection = (ExecutionCollection)collectionObj;

            var myIndex = collection.Index++;
            Action<object> operation = null;

            while (myIndex < collection.Length)
            {
                try
                {
                    operation = collection.Operations[myIndex];
                    var state = collection.UserStates[myIndex];
                    operation(state);
                }
                catch (Exception ex)
                {
                    HandleException(ex, operation, collection.Critical);
                }
                myIndex = collection.Index++;
            }
        }
        #endregion

        #region Try execute
        /// <summary>
        /// Try until condition is true and then execute
        /// </summary>
        public void TryExecute(Action operation, int maxTries, int waitTimeMs)
        {
            TryExecute(state => operation(), maxTries, waitTimeMs, null, false);
        }

        /// <summary>
        /// Try until condition is true and then execute
        /// </summary>
        public void TryExecute(Action operation, int maxTries, int waitTimeMs, bool criticalOperation)
        {
            TryExecute(state => operation(), maxTries, waitTimeMs, null, criticalOperation);
        }

        /// <summary>
        /// Try until condition is true and then execute
        /// </summary>
        public void TryExecute(Action<object> operation, int maxTries, int waitTimeMs, object operationState)
        {
            TryExecute(operation, maxTries, waitTimeMs, operationState, false);
        }

        /// <summary>
        /// Try until condition is true and then execute
        /// </summary>
        public void TryExecute(Action<object> operation, int maxTries, int waitTimeMs, object operationState, bool criticalOperation)
        {
            var context = new RetriedExecution
            {
                Operation = operation,
                OperationState = operation,
                MaxTries = maxTries,
            };
            context.TimerId = ScheduleExecution(TryExecution, context, 0, waitTimeMs);
        }

        private void TryExecution(object contextObj)
        {
            var context = (RetriedExecution)contextObj;
            try
            {
                context.Operation(context.OperationState);
                StopExecution(context.TimerId);
            }
            catch (Exception ex)
            {
                context.Tries++;
                if (context.Tries < context.MaxTries)
                    return;

                StopExecution(context.TimerId);
                throw new ImpossibleException("TryExecute exceeded the configured number of retries", ex);
            }
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
            return ScheduleExecution(state => operation(), null, delayMs, periodMs, false);
        }

        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        public int ScheduleExecution(Action operation, int delayMs, int periodMs, bool criticalOperation)
        {
            return ScheduleExecution(state => operation(), null, delayMs, periodMs, criticalOperation);
        }

        /// <summary>
        /// Execute non-critical operation periodically but non-stacking
        /// </summary>
        public int ScheduleExecution(Action<object> operation, object userState, int delayMs, int periodMs)
        {
            return ScheduleExecution(operation, userState, delayMs, periodMs, false);
        }

        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        public int ScheduleExecution(Action<object> operation, object userState, int delayMs, int periodMs, bool criticalOperation)
        {
            var id = ++_lastTimerId;
            var timer = new Timer(new NonStackingTimerCallback(state =>
            {
                try
                {
                    operation(state);
                    if(periodMs <= 0)
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

        /// <summary>
        /// Handle exception that occured in execution and would have killed the application
        /// </summary>
        private void HandleException(Exception ex, Delegate operation, bool criticalOperation)
        {
            if (criticalOperation)
                FailureReporting.ReportFailure(operation.Target, ex);
            else
                FailureReporting.ReportWarning(operation.Target, ex);
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
