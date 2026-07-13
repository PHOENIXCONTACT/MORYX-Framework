// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Threading;

/// <summary>
/// Extensions for the <see cref="IAsyncParallelOperations"/>
/// </summary>
public static class AsyncParallelOperationsExtensions
{
    /// <summary>
    /// Executes an async operation in parallel without blocking the caller.
    /// </summary>
    /// <param name="asyncParallelOperations">The extended component</param>
    /// <param name="operation">The async operation to execute</param>
    public static void ExecuteParallel(this IAsyncParallelOperations asyncParallelOperations, Func<Task> operation)
    {
        asyncParallelOperations.ExecuteParallel(operation, false);
    }

    /// <summary>
    /// Executes an async operation in parallel without blocking the caller.
    /// </summary>
    /// <param name="asyncParallelOperations">The extended component</param>
    /// <typeparam name="T">Type of the user state object</typeparam>
    /// <param name="operation">The async operation to execute</param>
    /// <param name="userState">User state passed to the operation</param>
    public static void ExecuteParallel<T>(this IAsyncParallelOperations asyncParallelOperations, Func<T, Task> operation, T userState)
        where T : class
    {
        asyncParallelOperations.ExecuteParallel(operation, userState, false);
    }

    /// <summary>
    /// Schedules an async operation to execute with a delay and optional periodic execution.
    /// Non-stacking: if an execution takes longer than the period, the next execution is skipped.
    /// </summary>
    /// <param name="asyncParallelOperations">The extended component</param>
    /// <param name="operation">The async operation to execute</param>
    /// <param name="delayMs">Initial delay in milliseconds before first execution</param>
    /// <param name="periodMs">Period in milliseconds for repeated execution. Set to 0 or negative for one-time execution</param>
    /// <returns>Unique identifier for the scheduled execution, used to stop it later</returns>
    public static int ScheduleExecution(this IAsyncParallelOperations asyncParallelOperations, Func<Task> operation,
        int delayMs, int periodMs)
    {
        return asyncParallelOperations.ScheduleExecution(operation, delayMs, periodMs, false);
    }

    /// <summary>
    /// Schedules an async operation to execute with a delay and optional periodic execution.
    /// Non-stacking: if an execution takes longer than the period, the next execution is skipped.
    /// </summary>
    /// <typeparam name="T">Type of the user state object</typeparam>
    /// <param name="asyncParallelOperations">The extended component</param> 
    /// <param name="operation">The async operation to execute</param>
    /// <param name="userState">User state passed to the operation</param>
    /// <param name="delayMs">Initial delay in milliseconds before first execution</param>
    /// <param name="periodMs">Period in milliseconds for repeated execution. Set to 0 or negative for one-time execution</param>
    /// <returns>Unique identifier for the scheduled execution, used to stop it later</returns>
    public static int ScheduleExecution<T>(this IAsyncParallelOperations asyncParallelOperations,
        Func<T, Task> operation, T userState, int delayMs, int periodMs) where T : class
    {
        return asyncParallelOperations.ScheduleExecution(operation, userState, delayMs, periodMs, false);
    }

    /// <summary>
    /// Schedules an async operation to execute with a delay and optional periodic execution.
    /// Non-stacking: if an execution takes longer than the period, the next execution is skipped.
    /// Use the provided cancellation token to stop the execution.
    /// </summary>
    /// <param name="asyncParallelOperations">The extended component</param> 
    /// <param name="operation">The async operation to execute</param>
    /// <param name="delayMs">Initial delay in milliseconds before first execution</param>
    /// <param name="periodMs">Period in milliseconds for repeated execution.   Set to 0 or negative for one-time execution</param>
    /// <param name="cancellationToken">Cancellation token to stop the scheduled execution</param>
    /// <returns>A task that completes when the scheduled execution is cancelled or completes</returns>
    public static Task ScheduleExecutionAsync(this IAsyncParallelOperations asyncParallelOperations, Func<Task> operation,
        int delayMs, int periodMs, CancellationToken cancellationToken = default)
    {
        return asyncParallelOperations.ScheduleExecutionAsync(operation, delayMs, periodMs, false, cancellationToken);
    }

    /// <summary>
    /// Schedules an async operation to execute with a delay and optional periodic execution.
    /// Non-stacking: if an execution takes longer than the period, the next execution is skipped.
    /// Use the provided cancellation token to stop the execution. 
    /// </summary>
    /// <typeparam name="T">Type of the user state object</typeparam>
    /// <param name="asyncParallelOperations">The extended component</param> 
    /// <param name="operation">The async operation to execute</param>
    /// <param name="userState">User state passed to the operation</param>
    /// <param name="delayMs">Initial delay in milliseconds before first execution</param>
    /// <param name="periodMs">Period in milliseconds for repeated execution.  Set to 0 or negative for one-time execution</param>
    /// <param name="cancellationToken">Cancellation token to stop the scheduled execution</param>
    /// <returns>A task that completes when the scheduled execution is cancelled or completes</returns>
    public static Task ScheduleExecutionAsync<T>(this IAsyncParallelOperations asyncParallelOperations,
        Func<T, Task> operation, T userState, int delayMs, int periodMs, CancellationToken cancellationToken = default) where T : class
    {
        return asyncParallelOperations.ScheduleExecutionAsync(operation, userState, delayMs, periodMs, false, cancellationToken);
    }
}
