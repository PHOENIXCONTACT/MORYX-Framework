using System;

namespace Marvin.Threading
{
    /// <summary>
    /// Interface for operations that should be executed parallely without redundant try catch
    /// </summary>
    public interface IParallelOperations : IDisposable
    {
        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        void ExecuteParallel(Action operation);
        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        void ExecuteParallel(Action operation, bool criticalOperation);
        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        void ExecuteParallel<T>(Action<T> operation, T userState) where T : class;
        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        void ExecuteParallel<T>(Action<T> operation, T userState, bool criticalOperation) where T : class;

        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        /// <returns>Timer id to stop periodic execution</returns>
        int ScheduleExecution(Action operation, int delayMs, int periodMs);
        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        /// <returns>Timer id to stop periodic execution</returns>
        int ScheduleExecution(Action operation, int delayMs, int periodMs, bool criticalOperation);
        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        /// <returns>Timer id to stop periodic execution</returns>
        int ScheduleExecution<T>(Action<T> operation, T userState, int delayMs, int periodMs) where T : class;
        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        /// <returns>Timer id to stop periodic execution</returns>
        int ScheduleExecution<T>(Action<T> operation, T userState, int delayMs, int periodMs, bool criticalOperation) where T : class;

        /// <summary>
        /// Stop the execution of the timer with this id
        /// </summary>
        /// <param name="timerId"></param>
        void StopExecution(int timerId);
    }
}
