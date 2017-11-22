using System;
using Marvin.Threading;

namespace Marvin.TestTools.UnitTest
{
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

        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        /// <returns>
        /// Timer id to stop periodic execution
        /// </returns>
        public int ScheduleExecution(Action operation, int delayMs, int periodMs)
        {
            throw new NotImplementedException($"{nameof(ScheduleExecution)} is not supported by {nameof(NotSoParallelOps)}");
        }

        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        /// <returns>
        /// Timer id to stop periodic execution
        /// </returns>
        public int ScheduleExecution(Action operation, int delayMs, int periodMs, bool criticalOperation)
        {
            throw new NotImplementedException($"{nameof(ScheduleExecution)} is not supported by {nameof(NotSoParallelOps)}");
        }

        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        /// <returns>
        /// Timer id to stop periodic execution
        /// </returns>
        public int ScheduleExecution<T>(Action<T> operation, T userState, int delayMs, int periodMs) where T : class
        {
            throw new NotImplementedException($"{nameof(ScheduleExecution)} is not supported by {nameof(NotSoParallelOps)}");
        }

        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        /// <returns>
        /// Timer id to stop periodic execution
        /// </returns>
        public int ScheduleExecution<T>(Action<T> operation, T userState, int delayMs, int periodMs, bool criticalOperation) where T : class
        {
            throw new NotImplementedException($"{nameof(ScheduleExecution)} is not supported by {nameof(NotSoParallelOps)}");
        }

        /// <summary>
        /// Stop the execution of the timer with this id
        /// </summary>
        /// <param name="timerId"/>
        public void StopExecution(int timerId)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}