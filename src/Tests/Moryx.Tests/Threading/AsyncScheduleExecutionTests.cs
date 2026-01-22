// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Moryx.Threading;
using NUnit.Framework;

namespace Moryx.Tests.Threading;

#pragma warning disable CA2201 // Do not raise reserved exception types

[TestFixture]
public class AsyncScheduleExecutionTests
{
    private AsyncParallelOperations _asyncParallelOperations;
    private AsyncParallelOperationsTestLogger _logger;

    [SetUp]
    public void Setup()
    {
        _logger = new AsyncParallelOperationsTestLogger();
        _asyncParallelOperations = new AsyncParallelOperations(_logger);
    }

    [TearDown]
    public void TearDown()
    {
        _asyncParallelOperations?.Dispose();
    }

    [Test(Description = "ScheduleExecution executes operation after delay.")]
    public async Task ExecutesAfterDelay()
    {
        // Arrange
        var executed = false;
        Task Operation()
        {
            executed = true;
            return Task.CompletedTask;
        }

        // Act
        var id = _asyncParallelOperations.ScheduleExecution(Operation, 100, 0, false);

        // Assert
        // Should not execute immediately
        Assert.That(executed, Is.False);

        // Wait for delay + execution
        await Task.Delay(200);
        Assert.That(executed, Is.True);
        Assert.That(id, Is.GreaterThan(0));
    }

    [Test(Description = "ScheduleExecution executes operation periodically delay.")]
    public async Task ExecutesPeriodically()
    {
        // Arrange
        var executionCount = 0;

        Task Operation()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - Execute every 100ms
        var id = _asyncParallelOperations.ScheduleExecution(Operation, 50, 100, false);

        // Assert - Wait for multiple executions
        await Task.Delay(450);
        _asyncParallelOperations.StopExecution(id);

        // Should execute approximately 4 times (50ms delay + 3-4 periods of 100ms in 450ms)
        Assert.That(executionCount, Is.GreaterThanOrEqualTo(3));
        Assert.That(executionCount, Is.LessThanOrEqualTo(5));
    }

    [TestCase(0, Description = "ScheduleExecution executes operation once if period is 0.")]
    [TestCase(-1, Description = "ScheduleExecution executes operation once if period is negative.")]
    public async Task ExecutesOnlyOnceWithZeroOrNegativePeriod(int period)
    {
        // Arrange
        var executionCount = 0;

        Task Operation()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act
        _asyncParallelOperations.ScheduleExecution(Operation, 50, period, false);

        // Assert
        await Task.Delay(200);
        Assert.That(executionCount, Is.EqualTo(1));
    }

    [Test(Description = "ScheduleExecution passes user-state to operation.")]
    public async Task UserStatePassedToOperation()
    {
        // Arrange
        var testState = new object();
        object receivedValue = null;

        Task Operation(object state)
        {
            receivedValue = state;
            return Task.CompletedTask;
        }

        // Act
        _asyncParallelOperations.ScheduleExecution(Operation, testState, 50, 0, false);

        // Assert
        await Task.Delay(150);
        Assert.That(receivedValue, Is.SameAs(testState));
    }

    [Test(Description = "ScheduleExecution skips execution if previous is execution is still running.")]
    public async Task SkipExecutionWhenPreviousStillRunning()
    {
        // Arrange
        var executionCount = 0;
        var inExecution = 0;

        async Task Operation()
        {
            Interlocked.Increment(ref inExecution);
            Interlocked.Increment(ref executionCount);

            // Long-running operation (200ms)
            await Task.Delay(200);

            Interlocked.Decrement(ref inExecution);
        }

        // Act
        // Period is 50ms but execution takes 200ms
        var id = _asyncParallelOperations.ScheduleExecution(Operation, 0, 50, false);

        // Assert
        // Wait for 400ms (should allow ~2 executions, not 8)
        await Task.Delay(400);
        _asyncParallelOperations.StopExecution(id);

        await Task.Delay(250); // Wait for last execution to finish

        // Should have executed 2-3 times max due to non-stacking
        Assert.That(executionCount, Is.LessThanOrEqualTo(3));
        Assert.That(inExecution, Is.EqualTo(0), "All executions should be complete");
    }

    [Test(Description = "StopExecution stops scheduled execution.")]
    public async Task StopPeriodicExecution()
    {
        // Arrange
        var executionCount = 0;

        Task Operation()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act
        var id = _asyncParallelOperations.ScheduleExecution(Operation,  50, 100, false);
        await Task.Delay(250); // Let it execute a few times

        var countBeforeStop = executionCount;
        _asyncParallelOperations.StopExecution(id);

        await Task.Delay(300); // Wait longer
        var countAfterStop = executionCount;

        // Assert
        Assert.That(countBeforeStop, Is.GreaterThanOrEqualTo(2));
        Assert.That(countAfterStop, Is.EqualTo(countBeforeStop), "Should not execute after stop");
    }

    [Test(Description = "StopExecution with invalid ids does not throw exceptions.")]
    public void StopExecutionWithInvalidIdDoesNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() => _asyncParallelOperations.StopExecution(999));
        Assert.DoesNotThrow(() => _asyncParallelOperations.StopExecution(-1));
    }

    [Test(Description = "ScheduleExecution catches exceptions of operation.")]
    public async Task CatchExceptionsOfOperation()
    {
        // Arrange
        static async Task Operation()
        {
            await Task.Delay(10);
            throw new InvalidOperationException("Test exception");
        }

        // Act
        _asyncParallelOperations.ScheduleExecution(Operation,  50, 0, false);

        // Assert
        await Task.Delay(150);
        Assert.That(_logger.HasErrors, Is.True);
        Assert.That(_logger.LastException, Is.TypeOf<InvalidOperationException>());
    }

    [Test(Description = "ScheduleExecution continues periodic execution after exception in an operation.")]
    public async Task ContinuesPeriodicExecutionAfterException()
    {
        // Arrange
        var executionCount = 0;

        async Task Operation()
        {
            Interlocked.Increment(ref executionCount);
            await Task.CompletedTask;
            throw new Exception("Test exception");
        }

        // Act
        var id = _asyncParallelOperations.ScheduleExecution(Operation, 50, 100, false);

        // Assert - Should continue despite exceptions
        await Task.Delay(450);
        _asyncParallelOperations.StopExecution(id);

        Assert.That(executionCount, Is.GreaterThanOrEqualTo(3), "Should continue executing despite exceptions");
        Assert.That(_logger.ErrorCount, Is.GreaterThanOrEqualTo(3), "All exceptions should be logged");
    }

    [Test(Description = "Multiple ScheduleExecution are executed independently.")]
    public async Task MultipleSchedulesExecuteIndependently()
    {
        // Arrange
        var count1 = 0;
        var count2 = 0;

        Task Operation1()
        {
            Interlocked.Increment(ref count1);
            return Task.CompletedTask;
        }

        Task Operation2()
        {
            Interlocked.Increment(ref count2);
            return Task.CompletedTask;
        }

        // Act
        var id1 = _asyncParallelOperations.ScheduleExecution(Operation1,  50, 100, false);
        var id2 = _asyncParallelOperations.ScheduleExecution(Operation2,  50, 150, false);

        // Assert
        await Task.Delay(500);
        _asyncParallelOperations.StopExecution(id1);
        _asyncParallelOperations.StopExecution(id2);

        Assert.That(count1, Is.GreaterThanOrEqualTo(4));
        Assert.That(count2, Is.GreaterThanOrEqualTo(2));
        Assert.That(count1, Is.GreaterThan(count2), "Faster period should execute more times");
    }

    [Test(Description = "ScheduleExecution with zero delay executes operation immediately.")]
    public async Task ZeroDelayExecutesImmediately()
    {
        // Arrange
        var executed = false;

        Task Operation()
        {
            executed = true;
            return Task.CompletedTask;
        }

        // Act
        _asyncParallelOperations.ScheduleExecution(Operation, 0, 0, false);

        // Assert
        await Task.Delay(100);
        Assert.That(executed, Is.True);
    }

    [Test(Description = "Dispose stops all scheduled executions.")]
    public async Task DisposeStopsAllScheduledExecutions()
    {
        // Arrange
        var executionCount = 0;
        Task Operation()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        _asyncParallelOperations.ScheduleExecution(Operation, 50, 100, false);
        _asyncParallelOperations.ScheduleExecution(Operation, 50, 100, false);

        await Task.Delay(200);
        var countBeforeDispose = executionCount;

        // Act
        _asyncParallelOperations.Dispose();
        await Task.Delay(300);
        var countAfterDispose = executionCount;

        // Assert
        Assert.That(countBeforeDispose, Is.GreaterThan(0));
        Assert.That(countAfterDispose, Is.EqualTo(countBeforeDispose), "Should not execute after dispose");
    }

    [Test(Description = "Cancel ScheduleExecutionAsync by CancellationToken")]
    public async Task CancelScheduleExecutionAsyncByCancellationToken()
    {
        // ReSharper disable MethodSupportsCancellation
        // Arrange
        var executionCount = 0;
        var cts = new CancellationTokenSource();

        Task Operation()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act
        var task = _asyncParallelOperations.ScheduleExecutionAsync(
            Operation, 50, 100, false, cts.Token);


        await Task.Delay(350);
        var countBeforeCancel = executionCount;

        await cts.CancelAsync();
        await task; // Wait for completion

        await Task.Delay(200);
        var countAfterCancel = executionCount;

        // Assert
        Assert.That(countBeforeCancel, Is.GreaterThanOrEqualTo(3));
        Assert.That(countAfterCancel, Is.EqualTo(countBeforeCancel));
    }

    [Test(Description = "Task of ScheduleExecutionAsync completes when CancellationToken was cancelled.")]
    public async Task ScheduleExecutionAsyncCompletesWhenCancelled()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        static Task Operation() => Task.Delay(10);

        // Act
        var task = _asyncParallelOperations.ScheduleExecutionAsync(
            Operation, 50, 100, false, cts.Token);

        await Task.Delay(200);
        await cts.CancelAsync();

        // Assert - Should complete without throwing
        await task;
        Assert.That(task.IsCompleted, Is.True);
    }

    [Test(Description = "ScheduleExecutionAsync stops automatically after timeout")]
    public async Task ScheduleExecutionAsyncStopsAutomaticallyWithTimeout()
    {
        // Arrange
        var executionCount = 0;
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(250));

        Task Operation()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act
        await _asyncParallelOperations.ScheduleExecutionAsync(
            Operation,  0, 50, false, cts.Token);

        // Assert - Should have executed multiple times but stopped due to timeout
        Assert.That(executionCount, Is.GreaterThanOrEqualTo(4));
        Assert.That(executionCount, Is.LessThanOrEqualTo(6));
    }
}
