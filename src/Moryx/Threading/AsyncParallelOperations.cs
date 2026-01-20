// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Moryx.Threading;

/// <summary>
/// Provides asynchronous parallel execution capabilities using the Task Parallel Library (TPL).
/// This interface offers methods to execute operations in parallel and schedule periodic executions
/// without blocking the calling thread. All operations are wrapped with exception handling and logging.
/// </summary>
public class AsyncParallelOperations : IAsyncParallelOperations, IDisposable
{
    private readonly ConcurrentDictionary<int, ScheduledExecutionContext> _runningSchedules = new();
    private readonly ConcurrentDictionary<Delegate, object> _eventDecouplers = new();
    private int _lastScheduleId;

    /// <summary>
    /// Dependency to report errors to plugin
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// Create parallel operations in service collection
    /// </summary>
    public AsyncParallelOperations(ILogger<ParallelOperations> logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Create parallel operations within module
    /// </summary>
    /// <param name="logger"></param>
    public AsyncParallelOperations(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Executes an async operation in parallel without blocking the caller. 
    /// </summary>
    public void ExecuteParallel(Func<Task> operation, bool criticalOperation)
    {
        // Fire and forget - start the task but don't wait for it
        _ = Task.Run(async () =>
        {
            try
            {
                await operation();
            }
            catch (Exception ex)
            {
                HandleException(ex, operation, criticalOperation);
            }
        });
    }

    /// <summary>
    /// Executes an async operation in parallel without blocking the caller.
    /// </summary>
    public void ExecuteParallel<T>(Func<T, Task> operation, T userState, bool criticalOperation) where T : class
    {
        // Delegate to the non-generic version using a closure
        ExecuteParallel(() => operation(userState), criticalOperation);
    }

    #region Scheduled Execution

    /// <inheritdoc />
    public int ScheduleExecution(Func<Task> operation, int delayMs, int periodMs, bool criticalOperation)
    {
        // Thread-safe increment of schedule id
        var id = Interlocked.Increment(ref _lastScheduleId);

        var context = new ScheduledExecutionContext
        {
            CancellationTokenSource = new CancellationTokenSource()
        };

        _runningSchedules[id] = context;

        // Start the scheduled execution on a background task
        _ = Task.Run(async () =>
        {
            await ExecuteScheduledAsync(id, operation, delayMs, periodMs, criticalOperation, context);
        }, context.CancellationTokenSource.Token);

        return id;
    }

    /// <inheritdoc />
    public int ScheduleExecution<T>(Func<T, Task> operation, T userState, int delayMs, int periodMs, bool criticalOperation) where T : class
    {
        // Wrap the generic operation in a non-generic lambda and delegate to the non-generic version
        return ScheduleExecution(() => operation(userState), delayMs, periodMs, criticalOperation);
    }

    private async Task ExecuteScheduledAsync(int id, Func<Task> operation, int delayMs, int periodMs, bool criticalOperation, ScheduledExecutionContext context)
    {
        var cancellationToken = context.CancellationTokenSource.Token;

        try
        {
            // Initial delay
            if (delayMs > 0)
            {
                await Task.Delay(delayMs, cancellationToken);
            }

            // Execute the operation (potentially multiple times if periodic)
            do
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                // Non-stacking execution:  only execute if previous execution is complete
                // Use Wait(0) for non-blocking check - returns true if semaphore was acquired
                if (await context.ExecutionSemaphore.WaitAsync(0, cancellationToken))
                {
                    try
                    {
                        await operation();
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex, operation, criticalOperation);
                    }
                    finally
                    {
                        context.ExecutionSemaphore.Release();
                    }
                }
                // else: previous execution still running, skip this iteration (non-stacking behavior)

                // If not periodic (periodMs <= 0), stop after first execution
                if (periodMs <= 0)
                {
                    break;
                }

                // Wait for the next period
                await Task.Delay(periodMs, cancellationToken);

            } while (periodMs > 0 && !cancellationToken.IsCancellationRequested);
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
        catch (Exception ex)
        {
            HandleException(ex, operation, criticalOperation);
        }
        finally
        {
            // Cleanup
            StopExecution(id);
        }
    }

    /// <inheritdoc />
    public void StopExecution(int id)
    {
        if (_runningSchedules.TryRemove(id, out var context))
        {
            context.CancellationTokenSource.Cancel();
            context.CancellationTokenSource.Dispose();
            context.ExecutionSemaphore?.Dispose();
        }
    }

    private class ScheduledExecutionContext
    {
        public CancellationTokenSource CancellationTokenSource { get; init; }

        public SemaphoreSlim ExecutionSemaphore { get; } = new(1, 1);
    }

    #endregion

    /// <inheritdoc />
    public void Dispose()
    {
        // Stop all running schedules
        foreach (var runningSchedule in _runningSchedules)
        {
            runningSchedule.Value.CancellationTokenSource.Cancel();
            runningSchedule.Value.CancellationTokenSource.Dispose();
            runningSchedule.Value.ExecutionSemaphore.Dispose();
        }
        _runningSchedules.Clear();
    }

    #region Event Decoupling

    /// <inheritdoc />
    public Func<object, TEventArgs, Task> DecoupleListener<TEventArgs>(Func<object, TEventArgs, Task> target)
    {
        var decoupler = CreateDecoupler(target);
        return decoupler.EventListener;
    }


    /// <summary>
    /// Create a typed async event decoupler for the given listener delegate
    /// </summary>
    private AsyncEventDecoupler<TEventArgs> CreateDecoupler<TEventArgs>(Func<object, TEventArgs, Task> target)
    {
        var decoupler = new AsyncEventDecoupler<TEventArgs>(target, this);

        lock (_eventDecouplers)
        {
            _eventDecouplers.TryAdd(target, decoupler);
        }

        return decoupler;
    }

    /// <inheritdoc />
    public Func<object, TEventArgs, Task> RemoveListener<TEventArgs>(Func<object, TEventArgs, Task> target)
    {
        var decoupler = RemoveDecoupler(target);
        return decoupler.EventListener;
    }

    /// <summary>
    /// Remove the decoupler from the collection and dispose it
    /// </summary>
    private AsyncEventDecoupler<TEventArgs> RemoveDecoupler<TEventArgs>(Func<object, TEventArgs, Task> target)
    {
        AsyncEventDecoupler<TEventArgs> decoupler;
        lock (_eventDecouplers)
        {
            if (!_eventDecouplers.TryGetValue(target, out var decouplerObj))
            {
                throw new InvalidOperationException("Can not remove a previously unregistered listener!");
            }

            decoupler = (AsyncEventDecoupler<TEventArgs>)decouplerObj;
            _eventDecouplers.TryRemove(target, out _);
        }
        return decoupler;
    }

    /// <summary>
    /// The <see cref="AsyncEventDecoupler{TEventArgs}"/> provides an async event listener that enqueues incoming calls
    /// and forwards them on a dedicated worker thread without blocking the event invocation
    /// </summary>
    /// <typeparam name="TEventArgs">Arguments for the async event handler</typeparam>
    private class AsyncEventDecoupler<TEventArgs>
    {
        private readonly Func<object, TEventArgs, Task> _eventTarget;
        private readonly IAsyncParallelOperations _parallelOperations;
        private readonly ConcurrentQueue<(object sender, TEventArgs args)> _eventQueue = new();
        private int _pendingEvents;

        public AsyncEventDecoupler(Func<object, TEventArgs, Task> eventTarget, IAsyncParallelOperations parallelOperations)
        {
            _eventTarget = eventTarget;
            _parallelOperations = parallelOperations;

            // Create the delegate once and store it
            EventListener = EventListenerMethod;
        }

        /// <summary>
        /// The event listener delegate that can be subscribed to events
        /// </summary>
        public Func<object, TEventArgs, Task> EventListener { get; }

        /// <summary>
        /// The actual event listener method
        /// </summary>
        private Task EventListenerMethod(object sender, TEventArgs eventArgs)
        {
            _eventQueue.Enqueue((sender, eventArgs));

            if (Interlocked.Increment(ref _pendingEvents) == 1)
                _parallelOperations.ExecuteParallel(ProcessEventQueue);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Process the event queue asynchronously
        /// </summary>
        private async Task ProcessEventQueue()
        {
            do
            {
                _eventQueue.TryDequeue(out var nextEvent);
                await _eventTarget(nextEvent.sender, nextEvent.args);
            }
            while (Interlocked.Decrement(ref _pendingEvents) > 0);
        }
    }

    #endregion

    private void HandleException(Exception ex, Func<Task> operation, bool criticalOperation)
    {
        var logger = ExtractLoggerFromDelegate(operation);
        logger.Log(criticalOperation ? LogLevel.Critical : LogLevel.Error, ex,
            "Exception during AsyncParallelOperations of {declaringType}.{methodName}!", operation.Method.DeclaringType?.Name ?? "Unknown", operation.Method.Name);
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

    
}
