using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Marvin.Logging;
using Marvin.TestTools.UnitTest;
using Marvin.Threading;
using NUnit.Compatibility;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;

namespace Marvin.Tests.Threading
{
    [TestFixture]
    public class EventDecouplerTests
    {
        private ParallelOperations _parallelOperations;

        [SetUp]
        public void Setup()
        {
            _parallelOperations = new ParallelOperations
            {
                Logger = new DummyLogger()
            };
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
            var listener = new EventHandler<EventArgs>((sender, args) => {});

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
            blockingEvent.WaitOne(1000);
        }

        #endregion

        #region ExceptionAndLogging

        private class InvocationTarget : ILoggingComponent
        {
            public IModuleLogger Logger { get; set; } = new DummyLogger();

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
            var logger = (DummyLogger)(targetHasLogger ? target.Logger : _parallelOperations.Logger);
            if (targetHasLogger)
                SimpleEventSource += _parallelOperations.DecoupleListener(target.FaultyListener);
            else
                SimpleEventSource += _parallelOperations.DecoupleListener(FaultyListener);
            // Act
            var cycles = 5;
            SimpleEventSource(this, EventArgs.Empty);
            while (!logger.Messages.Any() && cycles > 0)
            {
                Thread.Sleep(1);
                cycles--;
            }

            // Assert
            if (targetHasLogger)
                Assert.AreEqual(0, ((DummyLogger)_parallelOperations.Logger).Messages.Count, "ParallelOperations should not use its own logger for logging components");
            Assert.AreEqual(logger.Messages[0].Exception.Message, "Test", "Did not log the correct exception");
        }

        public void FaultyListener(object sender, EventArgs e)
        {
            throw new Exception("Test");
        }

        #endregion
    }
}