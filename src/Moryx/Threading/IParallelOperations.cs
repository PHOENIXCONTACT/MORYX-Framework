// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Threading;

/// <summary>
/// Interface for operations that should be executed in parallel without redundant try catch. 
/// Uses ThreadPool for execution and System.Threading.Timer for scheduled operations.
/// All operations are wrapped with exception handling and logging.
/// </summary>
public interface IParallelOperations : IDisposable
{
    /// <summary>
    /// Executes an operation in parallel without blocking the caller.
    /// </summary>
    /// <param name="operation">The operation to execute</param>
    void ExecuteParallel(Action operation);

    /// <summary>
    /// Executes an operation in parallel without blocking the caller. 
    /// </summary>
    /// <param name="operation">The operation to execute</param>
    /// <param name="criticalOperation">Indicates if this is a critical operation for exception handling</param>
    void ExecuteParallel(Action operation, bool criticalOperation);

    /// <summary>
    /// Executes an operation in parallel without blocking the caller.
    /// </summary>
    /// <typeparam name="T">Type of the user state object</typeparam>
    /// <param name="operation">The operation to execute</param>
    /// <param name="userState">User state passed to the operation</param>
    void ExecuteParallel<T>(Action<T> operation, T userState) where T : class;

    /// <summary>
    /// Executes an operation in parallel without blocking the caller.
    /// </summary>
    /// <typeparam name="T">Type of the user state object</typeparam>
    /// <param name="operation">The operation to execute</param>
    /// <param name="userState">User state passed to the operation</param>
    /// <param name="criticalOperation">Indicates if this is a critical operation for exception handling</param>
    void ExecuteParallel<T>(Action<T> operation, T userState, bool criticalOperation) where T : class;

    /// <summary>
    /// Schedules an operation to execute with a delay and optional periodic execution.
    /// Non-stacking: If an execution takes longer than the period, the next execution is skipped.
    /// </summary>
    /// <param name="operation">The operation to execute</param>
    /// <param name="delayMs">Initial delay in milliseconds before first execution</param>
    /// <param name="periodMs">Period in milliseconds for repeated execution. Set to 0 or negative for one-time execution</param>
    /// <returns>Timer id to stop periodic execution</returns>
    int ScheduleExecution(Action operation, int delayMs, int periodMs);

    /// <summary>
    /// Schedules an operation to execute with a delay and optional periodic execution.
    /// Non-stacking: If an execution takes longer than the period, the next execution is skipped.
    /// </summary>
    /// <param name="operation">The operation to execute</param>
    /// <param name="delayMs">Initial delay in milliseconds before first execution</param>
    /// <param name="periodMs">Period in milliseconds for repeated execution. Set to 0 or negative for one-time execution</param>
    /// <param name="criticalOperation">Indicates if this is a critical operation for exception handling</param>
    /// <returns>Timer id to stop periodic execution</returns>
    int ScheduleExecution(Action operation, int delayMs, int periodMs, bool criticalOperation);

    /// <summary>
    /// Schedules an operation to execute with a delay and optional periodic execution.
    /// Non-stacking: If an execution takes longer than the period, the next execution is skipped.
    /// </summary>
    /// <typeparam name="T">Type of the user state object</typeparam>
    /// <param name="operation">The operation to execute</param>
    /// <param name="userState">User state passed to the operation</param>
    /// <param name="delayMs">Initial delay in milliseconds before first execution</param>
    /// <param name="periodMs">Period in milliseconds for repeated execution. Set to 0 or negative for one-time execution</param>
    /// <returns>Timer id to stop periodic execution</returns>
    int ScheduleExecution<T>(Action<T> operation, T userState, int delayMs, int periodMs) where T : class;

    /// <summary>
    /// Schedules an operation to execute with a delay and optional periodic execution.
    /// Non-stacking: If an execution takes longer than the period, the next execution is skipped.
    /// </summary>
    /// <typeparam name="T">Type of the user state object</typeparam>
    /// <param name="operation">The operation to execute</param>
    /// <param name="userState">User state passed to the operation</param>
    /// <param name="delayMs">Initial delay in milliseconds before first execution</param>
    /// <param name="periodMs">Period in milliseconds for repeated execution. Set to 0 or negative for one-time execution</param>
    /// <param name="criticalOperation">Indicates if this is a critical operation for exception handling</param>
    /// <returns>Timer id to stop periodic execution</returns>
    int ScheduleExecution<T>(Action<T> operation, T userState, int delayMs, int periodMs, bool criticalOperation) where T : class;

    /// <summary>
    /// Stops a scheduled execution by its identifier.
    /// </summary>
    /// <param name="timerId">The timer identifier returned by ScheduleExecution</param>
    void StopExecution(int timerId);

    /// <summary>
    /// Decouples the event listener delegate from the event invocation thread.
    /// The returned delegate can be subscribed to events and will execute the target handler on a background thread.
    /// </summary>
    /// <param name="target">Target delegate to decouple from the event invocation thread</param>
    /// <returns>The decoupling event listener</returns>
    EventHandler DecoupleListener(EventHandler<EventArgs> target);

    /// <summary>
    /// Decouples the event listener delegate from the event invocation thread. 
    /// The returned delegate can be subscribed to events and will execute the target handler on a background thread. 
    /// </summary>
    /// <typeparam name="TEventArgs">Type of event args</typeparam>
    /// <param name="target">Target delegate to decouple from the event invocation thread</param>
    /// <returns>The decoupling event listener</returns>
    EventHandler<TEventArgs> DecoupleListener<TEventArgs>(EventHandler<TEventArgs> target);

    /// <summary>
    /// Removes the decoupled listener from tracking.
    /// Returns the same delegate instance for unsubscription from events.
    /// </summary>
    /// <param name="target">Target delegate that is removed as a listener</param>
    /// <returns>The decoupling event listener</returns>
    EventHandler RemoveListener(EventHandler<EventArgs> target);

    /// <summary>
    /// Removes the decoupled listener from tracking.
    /// Returns the same delegate instance for unsubscription from events.
    /// </summary>
    /// <typeparam name="TEventArgs">Type of event args</typeparam>
    /// <param name="target">Target delegate that is removed as a listener</param>
    /// <returns>The decoupling event listener</returns>
    EventHandler<TEventArgs> RemoveListener<TEventArgs>(EventHandler<TEventArgs> target);
}
