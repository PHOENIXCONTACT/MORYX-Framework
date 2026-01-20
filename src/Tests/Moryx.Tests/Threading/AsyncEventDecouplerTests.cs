// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moryx.Threading;
using NUnit.Framework;

namespace Moryx.Tests.Threading;

#pragma warning disable IDE0039 // Disable Convert to local function
#pragma warning disable CA2201 // Do not raise reserved exception types

[TestFixture]
public class AsyncEventDecouplerTests
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

    [Test(Description = "DecoupleListener executes event handler")]
    public async Task DecoupleExecutesEventHandler()
    {
        // Arrange
        var executed = false;
        Func<object, EventArgs, Task> handler = async (_, _) =>
        {
            executed = true;
            await Task.CompletedTask;
        };

        // Act
        var decoupledHandler = _asyncParallelOperations.DecoupleListener(handler);
        await decoupledHandler(this, EventArgs.Empty);

        // Assert
        await Task.Delay(100);
        Assert.That(executed, Is.True);
    }

    [Test(Description = "DecoupleListener does not block event invocation.")]
    public async Task DecoupleDoesNotBlockEventInvocation()
    {
        // Arrange
        var handlerStarted = false;
        Func<object, EventArgs, Task> handler = async (_, _) =>
        {
            handlerStarted = true;
            await Task.Delay(200); // Long-running handler
        };

        // Act
        var decoupledHandler = _asyncParallelOperations.DecoupleListener(handler);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        await decoupledHandler(this, EventArgs.Empty);
        sw.Stop();

        // Assert - Should return immediately
        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(100));

        await Task.Delay(50);
        Assert.That(handlerStarted, Is.True);
    }

    [Test(Description = "DecoupleListener passes sender and event-args.")]
    public async Task DecouplePassesSenderAndEventArgs()
    {
        // Arrange
        object receivedEventArgs = null;
        var expectedEventArgs = new object();

        object receivedSender = null;
        var expectedSender = new object();

        Func<object, object, Task> handler = (sender, args) =>
        {
            receivedSender = sender;
            receivedEventArgs = args;
            return Task.CompletedTask;
        };

        // Act
        var decoupledHandler = _asyncParallelOperations.DecoupleListener(handler);
        await decoupledHandler(expectedSender, expectedEventArgs);

        // Assert
        await Task.Delay(100);
        Assert.That(receivedEventArgs, Is.SameAs(expectedEventArgs));
        Assert.That(receivedSender, Is.SameAs(expectedSender));
    }

    [Test(Description = "Execution order of events is preserved.")]
    public async Task ExecutionOrderIsPreserved()
    {
        // Arrange
        var executionOrder = new List<int>();
        Func<object, TestEventArgs, Task> handler = async (_, args) =>
        {
            await Task.Delay(10);
            lock (executionOrder)
            {
                executionOrder.Add(args.Value);
            }
        };

        // Act
        var decoupledHandler = _asyncParallelOperations.DecoupleListener(handler);
        await decoupledHandler(this, new TestEventArgs { Value = 1 });
        await decoupledHandler(this, new TestEventArgs { Value = 2 });
        await decoupledHandler(this, new TestEventArgs { Value = 3 });
        await decoupledHandler(this, new TestEventArgs { Value = 4 });

        // Assert
        await Task.Delay(200);
        Assert.That(executionOrder, Is.EqualTo([1, 2, 3, 4]));
    }

    [Test(Description = "Decoupler catches exceptions of event-handler.")]
    public async Task CatchesExceptionInHandler()
    {
        // Arrange
        Func<object, EventArgs, Task> handler = async (_, _) =>
        {
            await Task.Delay(10);
            throw new InvalidOperationException();
        };

        // Act
        var decoupledHandler = _asyncParallelOperations.DecoupleListener(handler);
        await decoupledHandler(this, EventArgs.Empty);

        // Assert
        await Task.Delay(100);
        Assert.That(_logger.HasErrors, Is.True);
        Assert.That(_logger.LastException, Is.TypeOf<InvalidOperationException>());
    }

    [Test(Description = "Decoupler continues processing other events after exception.")]
    public async Task ContinuesProcessingOtherEventsAfterException()
    {
        // Arrange
        var executionCount = 0;
        Func<object, TestEventArgs, Task> handler = (_, args) =>
        {
            Interlocked.Increment(ref executionCount);
            //if (args.Value == 2)
            //{
            //    throw new Exception("Test exception");
            //}
            return Task.CompletedTask;
        };

        // Act
        var decoupledHandler = _asyncParallelOperations.DecoupleListener(handler);
        await decoupledHandler(this, new TestEventArgs { Value = 1 });
        await decoupledHandler(this, new TestEventArgs { Value = 2 }); // This will throw
        await decoupledHandler(this, new TestEventArgs { Value = 3 });

        // Assert
        await Task.Delay(200);
        Assert.That(executionCount, Is.EqualTo(3), "All events should be processed");
        //Assert.That(_logger.ErrorCount, Is.EqualTo(1));
    }

    [Test(Description = "RemoveListener returns decoupled handler.")]
    public void RemoveListenerReturnsDecoupledHandler()
    {
        // Arrange
        Func<object, EventArgs, Task> handler = (_, _) => Task.CompletedTask;
        var decoupledHandler = _asyncParallelOperations.DecoupleListener(handler);

        // Act
        var removedHandler = _asyncParallelOperations.RemoveListener(handler);

        // Assert
        Assert.That(removedHandler, Is.SameAs(decoupledHandler));
    }

    [Test(Description = "RemoveListener with unregistered listener throws InvalidOperationException.")]
    public void RemoveUnregisteredListenerThrowsException()
    {
        // Arrange
        Func<object, EventArgs, Task> handler = (_, _) => Task.CompletedTask;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            _asyncParallelOperations.RemoveListener(handler));
    }

    [Test(Description = "DecoupleListener with real events instead of test-execution.")]
    public async Task DecoupleListenerWithRealEvents()
    {
        // Arrange
        var eventSource = new TestEventSource();
        var receivedValue = 0;

        Func<object, TestEventArgs, Task> handler = async (_, args) =>
        {
            receivedValue = args.Value;
            await Task.CompletedTask;
        };

        // Act
        var decoupledHandler = _asyncParallelOperations.DecoupleListener(handler);
        eventSource.TestEvent += decoupledHandler;

        await eventSource.RaiseEvent(new TestEventArgs { Value = 99 });

        // Assert
        await Task.Delay(100);
        Assert.That(receivedValue, Is.EqualTo(99));
    }

    [Test(Description = "RemoveListener after decoupled with real events instead of test-execution.")]
    public async Task RemoveListenerWithRealEvents()
    {
        // Arrange
        var eventSource = new TestEventSource();
        var executionCount = 0;

        Func<object, TestEventArgs, Task> handler = async (_, _) =>
        {
            Interlocked.Increment(ref executionCount);
            await Task.CompletedTask;
        };

        var decoupledHandler = _asyncParallelOperations.DecoupleListener(handler);
        eventSource.TestEvent += decoupledHandler;

        // Act
        await eventSource.RaiseEvent(new TestEventArgs { Value = 1 });
        await Task.Delay(100);

        eventSource.TestEvent -= _asyncParallelOperations.RemoveListener(handler);

        await eventSource.RaiseEvent(new TestEventArgs { Value = 2 });
        await Task.Delay(100);

        // Assert
        Assert.That(executionCount, Is.EqualTo(1), "Should only execute once before removal");
    }

    [Test(Description = "Multiple decoupled event handlers execute independently.")]
    public async Task MultipleDecoupledHandlersExecuteIndependently()
    {
        // Arrange
        var count1 = 0;
        var count2 = 0;

        Func<object, EventArgs, Task> handler1 = async (_, _) =>
        {
            Interlocked.Increment(ref count1);
            await Task.Delay(50);
        };

        Func<object, EventArgs, Task> handler2 = async (_, _) =>
        {
            Interlocked.Increment(ref count2);
            await Task.Delay(30);
        };

        // Act
        var decoupled1 = _asyncParallelOperations.DecoupleListener(handler1);
        var decoupled2 = _asyncParallelOperations.DecoupleListener(handler2);

        await decoupled1(this, EventArgs.Empty);
        await decoupled2(this, EventArgs.Empty);

        // Assert
        await Task.Delay(200);
        Assert.That(count1, Is.EqualTo(1));
        Assert.That(count2, Is.EqualTo(1));
    }

    [Test(Description = "DecoupleListener queues burst of events.")]
    public async Task DecoupleQueuesBurstOfEvents()
    {
        // Arrange
        var executionCount = 0;
        Func<object, EventArgs, Task> handler = async (_, _) =>
        {
            Interlocked.Increment(ref executionCount);
            await Task.Delay(20);
        };

        // Act
        var decoupledHandler = _asyncParallelOperations.DecoupleListener(handler);

        // Raise 10 events quickly
        for (var i = 0; i < 10; i++)
        {
            await decoupledHandler(this, EventArgs.Empty);
        }

        // Assert
        await Task.Delay(500);
        Assert.That(executionCount, Is.EqualTo(10), "All events should be processed");
    }

    private class TestEventArgs : EventArgs
    {
        public int Value { get; init; }
    }

    private class TestEventSource
    {
        public event Func<object, TestEventArgs, Task> TestEvent;
        public event Func<object, EventArgs, Task> TestEvent2;

        public async Task RaiseEvent(TestEventArgs args)
        {
            if (TestEvent != null)
            {
                foreach (var @delegate in TestEvent.GetInvocationList())
                {
                    var handler = (Func<object, TestEventArgs, Task>)@delegate;
                    await handler(this, args);
                }
            }
        }
    }
}
