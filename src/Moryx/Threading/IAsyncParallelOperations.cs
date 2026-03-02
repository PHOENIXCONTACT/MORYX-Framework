// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Threading;

/// <summary>
/// Provides asynchronous parallel execution capabilities using the Task Parallel Library (TPL).
/// This interface offers methods to execute operations in parallel and schedule periodic executions
/// without blocking the calling thread. All operations are wrapped with exception handling and logging.
/// </summary>
public interface IAsyncParallelOperations
{
    /// <summary>
    /// Executes an async operation in parallel without blocking the caller.
    /// </summary>
    /// <param name="operation">The async operation to execute</param>
    /// <param name="criticalOperation">Indicates if this is a critical operation for exception handling</param>
    void ExecuteParallel(Func<Task> operation, bool criticalOperation);

    /// <summary>
    /// Executes an async operation in parallel without blocking the caller.
    /// </summary>
    /// <typeparam name="T">Type of the user state object</typeparam>
    /// <param name="operation">The async operation to execute</param>
    /// <param name="userState">User state passed to the operation</param>
    /// <param name="criticalOperation">Indicates if this is a critical operation for exception handling</param>
    void ExecuteParallel<T>(Func<T, Task> operation, T userState, bool criticalOperation) where T : class;

    #region #region ID-based ScheduleExecution

    /// <summary>
    /// Schedules an async operation to execute with a delay and optional periodic execution.
    /// Non-stacking: if an execution takes longer than the period, the next execution is skipped.
    /// </summary>
    /// <param name="operation">The async operation to execute</param>
    /// <param name="delayMs">Initial delay in milliseconds before first execution</param>
    /// <param name="periodMs">Period in milliseconds for repeated execution. Set to 0 or negative for one-time execution</param>
    /// <param name="criticalOperation">Indicates if this is a critical operation for exception handling</param>
    /// <returns>Unique identifier for the scheduled execution, used to stop it later</returns>
    int ScheduleExecution(Func<Task> operation, int delayMs, int periodMs, bool criticalOperation);

    /// <summary>
    /// Schedules an async operation to execute with a delay and optional periodic execution.
    /// Non-stacking: if an execution takes longer than the period, the next execution is skipped.
    /// </summary>
    /// <typeparam name="T">Type of the user state object</typeparam>
    /// <param name="operation">The async operation to execute</param>
    /// <param name="userState">User state passed to the operation</param>
    /// <param name="delayMs">Initial delay in milliseconds before first execution</param>
    /// <param name="periodMs">Period in milliseconds for repeated execution. Set to 0 or negative for one-time execution</param>
    /// <param name="criticalOperation">Indicates if this is a critical operation for exception handling</param>
    /// <returns>Unique identifier for the scheduled execution, used to stop it later</returns>
    int ScheduleExecution<T>(Func<T, Task> operation, T userState, int delayMs, int periodMs, bool criticalOperation)
        where T : class;

    /// <summary>
    /// Stops a scheduled execution by its identifier. 
    /// </summary>
    /// <param name="id">The identifier returned by ScheduleExecution</param>
    void StopExecution(int id);

    #endregion

    #region Token-based ScheduleExecution

    /// <summary>
    /// Schedules an async operation to execute with a delay and optional periodic execution.
    /// Non-stacking: if an execution takes longer than the period, the next execution is skipped.
    /// Use the provided cancellation token to stop the execution.
    /// </summary>
    /// <param name="operation">The async operation to execute</param>
    /// <param name="delayMs">Initial delay in milliseconds before first execution</param>
    /// <param name="periodMs">Period in milliseconds for repeated execution.   Set to 0 or negative for one-time execution</param>
    /// <param name="criticalOperation">Indicates if this is a critical operation for exception handling</param>
    /// <param name="cancellationToken">Cancellation token to stop the scheduled execution</param>
    /// <returns>A task that completes when the scheduled execution is cancelled or completes</returns>
    Task ScheduleExecutionAsync(Func<Task> operation, int delayMs, int periodMs, bool criticalOperation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedules an async operation to execute with a delay and optional periodic execution.
    /// Non-stacking: if an execution takes longer than the period, the next execution is skipped.
    /// Use the provided cancellation token to stop the execution. 
    /// </summary>
    /// <typeparam name="T">Type of the user state object</typeparam>
    /// <param name="operation">The async operation to execute</param>
    /// <param name="userState">User state passed to the operation</param>
    /// <param name="delayMs">Initial delay in milliseconds before first execution</param>
    /// <param name="periodMs">Period in milliseconds for repeated execution.  Set to 0 or negative for one-time execution</param>
    /// <param name="criticalOperation">Indicates if this is a critical operation for exception handling</param>
    /// <param name="cancellationToken">Cancellation token to stop the scheduled execution</param>
    /// <returns>A task that completes when the scheduled execution is cancelled or completes</returns>
    Task ScheduleExecutionAsync<T>(Func<T, Task> operation, T userState, int delayMs, int periodMs, bool criticalOperation, CancellationToken cancellationToken = default)
        where T : class;

    #endregion

    /// <summary>
    /// Decouple an async event listener delegate from the event invocation thread. 
    /// The returned delegate can be subscribed to events and will execute the target handler asynchronously.
    /// </summary>
    /// <typeparam name="TEventArgs">Type of event args</typeparam>
    /// <param name="target">Target async delegate to decouple</param>
    /// <returns>The decoupling event listener delegate</returns>
    Func<object, TEventArgs, Task> DecoupleListener<TEventArgs>(Func<object, TEventArgs, Task> target);

    /// <summary>
    /// Remove the decoupled async listener from tracking.
    /// Returns the same delegate instance for unsubscription from events.
    /// </summary>
    /// <typeparam name="TEventArgs">Type of event args</typeparam>
    /// <param name="target">Target async delegate that was decoupled</param>
    /// <returns>The decoupling event listener delegate</returns>
    Func<object, TEventArgs, Task> RemoveListener<TEventArgs>(Func<object, TEventArgs, Task> target);
}
