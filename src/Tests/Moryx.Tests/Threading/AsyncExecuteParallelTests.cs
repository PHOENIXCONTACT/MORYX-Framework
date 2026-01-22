// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moryx.Threading;
using NUnit.Framework;

namespace Moryx.Tests.Threading;

#pragma warning disable CA2201 // Do not raise reserved exception types

[TestFixture]
public class AsyncExecuteParallelTests
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

    #region ExecuteParallel Tests

    [Test(Description = "ExecuteParallel executes operation in background.")]
    public async Task ExecutesOperationInBackground()
    {
        // Arrange
        var executed = false;

        async Task Operation()
        {
            await Task.Delay(10);
            executed = true;
        }

        // Act
        _asyncParallelOperations.ExecuteParallel(Operation, false);

        // Assert 
        await Task.Delay(100);
        Assert.That(executed, Is.True);
    }


    [Test(Description = "ExecuteParallel passes user-state to operation.")]
    public async Task PassesUserStateToOperation()
    {
        // Arrange
        var testState = new object();
        object receivedState = null;

        async Task Operation(object state)
        {
            await Task.Delay(10);
            receivedState = state;
        }

        // Act
        _asyncParallelOperations.ExecuteParallel(Operation, testState, false);

        // Assert
        await Task.Delay(100);
        Assert.That(receivedState, Is.Not.Null);
        Assert.That(receivedState, Is.SameAs(testState));
    }

    [Test(Description = "ExecuteParallel catches exceptions of operation, and writes to log.")]
    public async Task CatchExceptionsOfOperation()
    {
        // Arrange
        const string exceptionMessage = "Test exception";
        static async Task Operation()
        {
            await Task.Delay(10);
            throw new InvalidOperationException(exceptionMessage);
        }

        // Act
        _asyncParallelOperations.ExecuteParallel(Operation, false);

        // Assert - Should not throw, exception should be logged
        await Task.Delay(100);
        Assert.That(_logger.HasErrors, Is.True);
        Assert.That(_logger.LastException, Is.TypeOf<InvalidOperationException>());
        Assert.That(_logger.LastException.Message, Is.EqualTo(exceptionMessage));
    }

    [Test(Description = "ExecuteParallel logs critical exception if critical-operation is enabled.")]
    public async Task LogCriticalExceptionWhenCriticalOperationIsTrue()
    {
        // Arrange
        static async Task Operation()
        {
            await Task.Delay(10);
            throw new Exception("Critical error");
        }

        // Act
        _asyncParallelOperations.ExecuteParallel(Operation, true);

        // Assert
        await Task.Delay(100);
        Assert.That(_logger.HasCriticalErrors, Is.True);
        Assert.That(_logger.LastLogLevel, Is.EqualTo(LogLevel.Critical));
    }

    [Test(Description = "ExecuteParallel logs exception if critical-operation is disabled.")]
    public async Task LogExceptionWhenCriticalOperationIsFalse()
    {
        // Arrange
        static async Task Operation()
        {
            await Task.Delay(10);
            throw new Exception("Non-critical error");
        }

        // Act
        _asyncParallelOperations.ExecuteParallel(Operation, false);

        // Assert
        await Task.Delay(100);
        Assert.That(_logger.HasErrors, Is.True);
        Assert.That(_logger.LastLogLevel, Is.EqualTo(LogLevel.Error));
    }

    [Test(Description = "ExecuteParallel executes multiple operations in parallel.")]
    public void ExecutesMultipleOperationsInParallel()
    {
        // Arrange
        var counter = 0;
        var operationsCount = 10;
        var countdown = new CountdownEvent(operationsCount);

        var operation = async () =>
        {
            await Task.Delay(50);
            Interlocked.Increment(ref counter);
            countdown.Signal();
        };

        // Act
        for (var i = 0; i < operationsCount; i++)
        {
            _asyncParallelOperations.ExecuteParallel(operation, false);
        }

        // Assert
        var completed = countdown.Wait(2000);
        Assert.That(completed, Is.True, "Not all operations completed in time");
        Assert.That(counter, Is.EqualTo(operationsCount));
    }

    [Test(Description = "ExecuteParallel executed operation if user-state is null.")]
    public async Task UserStateCanBeNull()
    {
        // Arrange
        object receivedState = null;

        async Task Operation(object state)
        {
            await Task.Delay(10);
            receivedState = state;
        }

        // Act
        _asyncParallelOperations.ExecuteParallel(Operation, (object)null, false);

        // Assert
        await Task.Delay(100);
        Assert.That(receivedState, Is.Null);
    }

    [Test(Description = "Multiple calls to ExecuteParallel: Each catches its own exception.")]
    public async Task MultipleOperationsEachCatchesItsOwnException()
    {
        // Arrange
        var successExecuted = false;
        var exceptionCount = 0;

        async Task SuccessOperation()
        {
            await Task.Delay(10);
            successExecuted = true;
        }

        async Task FailingOperation()
        {
            await Task.Delay(10);
            Interlocked.Increment(ref exceptionCount);
            throw new Exception("Operation failed");
        }

        // Act
        _asyncParallelOperations.ExecuteParallel(FailingOperation, false);
        _asyncParallelOperations.ExecuteParallel(SuccessOperation, false);
        _asyncParallelOperations.ExecuteParallel(FailingOperation, false);

        // Assert
        await Task.Delay(200);
        Assert.That(successExecuted, Is.True, "Success operation should have executed");
        Assert.That(exceptionCount, Is.EqualTo(2), "Both failing operations should have thrown");
        Assert.That(_logger.ErrorCount, Is.EqualTo(2), "Both exceptions should have been logged");
    }

    #endregion
}
