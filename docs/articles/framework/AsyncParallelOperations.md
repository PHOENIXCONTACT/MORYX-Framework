# AsyncParallelOperations

`AsyncParallelOperations` is a modern async/await-based component that provides parallel execution capabilities using the Task Parallel Library (TPL). It is designed as an asynchronous alternative to the traditional ThreadPool-based [`IParallelOperations`](ParallelOperations.md) component.

## Purpose

The primary purpose of `AsyncParallelOperations` is to:

- Support async/await patterns throughout your application
- Schedule periodic tasks with non-stacking behavior
- Handle exceptions automatically with built-in logging
- Decouple execution from the calling thread without blocking
- Decouple event handlers from event invocation threads

## Why AsyncParallelOperations instead of ParallelOperations?

1. Native async/await support - Leverages TPL for better integration with modern C# code
2. Better cancellation - Uses `CancellationToken` for clean cancellation of scheduled operations
3. Resource efficiency - Uses `Task.Delay` instead of blocking threads with `Timer`
4. Improved debugging - Async stack traces are more informative
5. Consistent pattern - Follows established async/await conventions

## Key Features

1. Execute operations in parallel without blocking the caller.
2. Schedule operations to run after a delay and optionally repeat at intervals.
3. If a scheduled operation takes longer than its period, subsequent executions are skipped until the current one completes.
4. Decouple async event handlers from the event invocation thread, ensuring events don't block callers.
5. All operations are wrapped with try-catch blocks and exceptions are logged automatically.

## Resource Cleanup

### Important: Event Handler Cleanup

When using event decoupling, the caller **must** unsubscribe event handlers. The caller is responsible for unsubscribing:

````cs
// Create by your own or by injection
private IAsyncParallelOperations _asyncParallelOperations;

// Subscribe
var handler = asyncOps. DecoupleListener<DataEventArgs>(OnDataAsync, false);
eventSource.DataReceived += handler;

// ...  use the event ...

// IMPORTANT: Unsubscribe
eventSource.DataReceived -= asyncOps.RemoveListener<DataEventArgs>(OnDataAsync);
````

## Examples

### Fire-and-Forget Execution

Execute an operation in the background without blocking:

````cs
// Create by your own or by injection
private IAsyncParallelOperations _asyncParallelOperations;

// Simple fire-and-forget
_asyncParallelOperations.ExecuteParallel(async () =>
{
    await Task. Delay(100);
    Console.WriteLine("Background operation completed");
}, criticalOperation: false);

// Continue immediately - doesn't wait for the operation
Console.WriteLine("Main thread continues.. .");
````

### Scheduled One-Time Execution

Execute an operation once after a delay:

````cs
// Create by your own or by injection
private IAsyncParallelOperations _asyncParallelOperations;

// Execute once after 5 seconds
int scheduleId = _asyncParallelOperations.ScheduleExecution(async () =>
{
    await PerformMaintenanceAsync();
    Console.WriteLine("Maintenance completed");
}, delayMs: 5000, periodMs: 0, criticalOperation: false);
````

### Scheduled Periodic Execution

Execute an operation periodically:

````cs
// Create by your own or by injection
private IAsyncParallelOperations _asyncParallelOperations;

// Execute every 10 seconds, starting after 2 seconds
int scheduleId = _asyncParallelOperations.ScheduleExecution(async () =>
{
    await CheckHealthAsync();
    Console.WriteLine($"Health check at {DateTime.Now}");
}, delayMs: 2000, periodMs: 10000, criticalOperation: false);

// Later...  stop the periodic execution
asyncOps.StopExecution(scheduleId);
````

### Schedule Periodic Execution with CancellationToken

Schedule periodic execution with usage of cancellation token:

````cs
// Create by your own or by injection
private IAsyncParallelOperations _asyncParallelOperations;

// Start
var cts = new CancellationTokenSource();
var task = _asyncParallelOperations.ScheduleExecutionAsync(async () =>
{
    await DoHealthCheckAsync();
}, delayMs: 1000, periodMs: 5000, criticalOperation: false, cts.Token);

// Stop and wait
await cts.CancelAsync();
await task;
````

### Event Decoupling

Decouple event handlers from the event invocation thread:

````cs
// Create by your own or by injection
private IAsyncParallelOperations _asyncParallelOperations;

// Define async event handler
async Task OnDataReceivedAsync(object sender, DataEventArgs e)
{
    // Long-running async operation
    await ProcessDataAsync(e.Data);
    await SaveToDatabase(e.Data);
}

// Decouple the handler
var decoupledHandler = asyncOps.DecoupleListener<DataEventArgs>(OnDataReceivedAsync);

// Subscribe to event
dataSource.DataReceived += decoupledHandler;

// Event invocation returns immediately, handler runs in background
await dataSource.RaiseDataReceivedEvent(new DataEventArgs { Data = "test" });
Console.WriteLine("Event raised, main thread continues...");

// Later... unsubscribe
dataSource.DataReceived -= asyncOps.RemoveListener<DataEventArgs>(OnDataReceivedAsync);
````
