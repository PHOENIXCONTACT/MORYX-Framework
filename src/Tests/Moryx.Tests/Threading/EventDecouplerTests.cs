// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.Logging;
using Moryx.Threading;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Moryx.Tests.Threading
{
    [TestFixture]
    public class EventDecouplerTests
    {
        private ParallelOperations _parallelOperations;
        private Tuple<LogLevel, string, Exception> _message;

        [SetUp]
        public void Setup()
        {
            var logger = new ModuleLogger("Dummy", new NullLoggerFactory(), (l, m, e) => _message = new(l, m, e));

            _message = null;

            _parallelOperations = new ParallelOperations(logger);
        }

        [TearDown]
        public void Dispose()
        {
            _parallelOperations.Dispose();
        }

        [Test]
        public void CanBeRemoved()
        {
            // Arrange
            var listener = new EventHandler<EventArgs>((sender, args) => { });

            // Act
            SimpleEventSource += _parallelOperations.DecoupleListener(listener);
            SimpleEventSource -= _parallelOperations.RemoveListener(listener);

            // Assert
            Assert.IsNull(SimpleEventSource);
        }


        #region Event without Arguments

        private EventArgs _receivedArgument;
        private event EventHandler SimpleEventSource;

        [Test]
        public void SupportsNonGenericEvents()
        {
            // Arrange
            SimpleEventSource += _parallelOperations.DecoupleListener(ArgumentlessTarget);

            // Act
            var cycles = 5;
            SimpleEventSource(this, EventArgs.Empty);
            while (_receivedArgument == null && cycles > 0)
            {
                Thread.Sleep(1);
                cycles--;
            }

            // Assert
            Assert.AreEqual(EventArgs.Empty, _receivedArgument);
        }

        private void ArgumentlessTarget(object sender, EventArgs args)
        {
            _receivedArgument = args;
        }

        #endregion

        #region Non-blocking behavior

        private event EventHandler<ManualResetEvent> ResetEventSource;

        [Test(Description = "Long running event handlers do not block event invocation")]
        public void ListenerDoesNotBlockInvocation()
        {
            // Arrange
            var resetEvent = new ManualResetEvent(false);
            ResetEventSource += _parallelOperations.DecoupleListener<ManualResetEvent>(BlockingTarget);

            // Act
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            ResetEventSource(this, resetEvent);
            ResetEventSource(this, resetEvent);
            stopWatch.Stop();
            resetEvent.Set();

            // Assert
            Assert.Greater(100, stopWatch.ElapsedMilliseconds);
        }

        private void BlockingTarget(object sender, ManualResetEvent blockingEvent)
        {
            blockingEvent.WaitOne(250);
        }

        #endregion

        #region ExceptionAndLogging

        private class InvocationTarget : ILoggingComponent
        {
            public Tuple<LogLevel, string, Exception> Message { get; set; }

            public IModuleLogger Logger { get; set; } 

            public InvocationTarget()
            {
                var logger = new ModuleLogger("Dummy", new NullLoggerFactory(), (l, m, e) => Message = new(l, m, e));
                Logger = logger;
            }

            public void FaultyListener(object sender, EventArgs e)
            {
                throw new Exception("Test");
            }
        }

       [TestCase(true, Description = "The handler raises throws an exception and is a logging component")]
       [TestCase(false, Description = "The handler raises throws an exception and is a logging component")]
        public void ExceptionInHandler(bool targetHasLogger)
        {
            // Arrange
            var target = new InvocationTarget();
            if (targetHasLogger)
                SimpleEventSource += _parallelOperations.DecoupleListener(target.FaultyListener);
            else
                SimpleEventSource += _parallelOperations.DecoupleListener(FaultyListener);
            // Act
            SimpleEventSource(this, EventArgs.Empty);
            while (target.Message == null && _message == null)
            {
                Thread.Sleep(1);
            }

            // Assert
            if (targetHasLogger)
            {
                Assert.IsNull(_message, "ParallelOperations should not use its own logger for logging components");
                Assert.NotNull(target.Message.Item3, "Did not log the correct exception");
            }
            else
            {
                Assert.NotNull(_message.Item3, "Did not log the correct exception");
            }
        }

        public void FaultyListener(object sender, EventArgs e)
        {
            throw new Exception("Test");
        }

        #endregion

        #region Multi-threaded and ordered

        private const int MaxIndex = 100;

        private event EventHandler<int> IndexEvent; 

        [Test]
        public void WriteFromMultipleThreads()
        {
            // Arrange
            var index = 0;
            IndexEvent += _parallelOperations.DecoupleListener<int>(IndexListener);

            // Act
            // 8 Threads write to the event with increasing numbers
            for (int run = 4; run >= 1; run--)
            {
                for (int thread = 1; thread <= 8; thread++)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        int next;
                        do
                        {
                            next = Interlocked.Increment(ref index);
                            IndexEvent(this, next);

                        } while (next < MaxIndex / run);
                    });
                }
                // Give the queue time to catch up
                var runFragment = MaxIndex / run;
                while (_count < (index > runFragment ? index : runFragment))
                    Thread.Sleep(1);
            }

            // Assert
            var expected = Enumerable.Range(1, index).Sum();
            Assert.AreEqual(expected, _total, "Sums do not match");
        }

        private int _count;

        private int _total;

        private void IndexListener(object sender, int index)
        {
            // Increment in multiple steps to increase the risk of overlap
            var value = _total;
            Thread.Sleep(1);
            var newValue = value + index;
            Thread.Sleep(1);
            _total = newValue;
            
            _count++;
        }

        #endregion
    }
}
