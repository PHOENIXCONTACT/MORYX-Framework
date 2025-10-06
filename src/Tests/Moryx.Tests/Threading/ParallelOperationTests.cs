// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Diagnostics;
using System.Threading;
using Moryx.Logging;
using Moryx.Threading;
using NUnit.Framework;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

namespace Moryx.Tests.Threading
{
    [TestFixture]
    public class ParallelOperationTests
    {
        private const string ExceptionMsg = "Hello World!";
        private const int MaxTrows = 3;
        private const int SleepTime = 1000;

        private ParallelOperations _threadFactory;
        private readonly ManualResetEventSlim _callbackReceivedEvent = new(false);

        private ModuleLogger _logger;
        private Tuple<LogLevel, string, Exception> _message;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new ModuleLogger("Dummy", new NullLoggerFactory(), (l, m, e) => _message = new(l, m, e));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }

        [SetUp]
        public void Setup()
        {
            _threadFactory = new ParallelOperations(_logger);

            _callbackReceivedEvent.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _threadFactory.Dispose();
            _message = null;
        }

        [Test]
        public void ExecuteParallel()
        {
            StateObject state = new StateObject();

            _threadFactory.ExecuteParallel(SimpleCallback, state);

            _callbackReceivedEvent.Wait(50);

            Assert.That(_callbackReceivedEvent.IsSet, "Callback not called.");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ExecuteParallelWithException(bool critical)
        {
            StateObject state = new StateObject();

            _threadFactory.ExecuteParallel(ExceptionCallback, state, critical);

            AwaitLogMessage();

            Assert.That(_message.Item1 == LogLevel.Critical, Is.EqualTo(critical), "Failure received");
            Assert.That(_message.Item1 == LogLevel.Error, Is.EqualTo(!critical), "Warning received");
        }

        [Ignore("Test fails because of timing issue on different system")]
        [Test]
        public void ScheduleExecutionWithStop()
        {
            StateObject state = new StateObject();

            int id = _threadFactory.ScheduleExecution(SimpleCallback, state, 100, 50);

            Thread.Sleep(75);

            Assert.That(state.Counter, Is.EqualTo(0), "First check");

            Thread.Sleep(50);

            Assert.That(state.Counter, Is.EqualTo(1), "Second check");

            Thread.Sleep(50);

            Assert.That(state.Counter, Is.EqualTo(2), "Third check");

            _threadFactory.StopExecution(id);

            Thread.Sleep(50);

            Assert.That(state.Counter, Is.EqualTo(2), "Last check");
        }

        [Test]
        public void ScheduleExecutionWithWrongStop()
        {
            StateObject state = new StateObject();

            int id = _threadFactory.ScheduleExecution(SimpleCallback, state, 200, 100);

            Thread.Sleep(150);

            Assert.That(state.Counter, Is.EqualTo(0), "First check");

            Thread.Sleep(100);

            Assert.That(state.Counter, Is.EqualTo(1), "Second check");

            Thread.Sleep(100);

            Assert.That(state.Counter, Is.EqualTo(2), "Third check");

            _threadFactory.StopExecution(42);

            Thread.Sleep(100);

            Assert.That(state.Counter, Is.EqualTo(3), "Last check");
        }

        [Ignore("Test fails because of timing issue on different system")]
        [Test]
        public void ScheduleExecutionWithDispose()
        {
            StateObject state = new StateObject();

            _threadFactory.ScheduleExecution(SimpleCallback, state, 100, 50);

            Thread.Sleep(75);

            Assert.That(state.Counter, Is.EqualTo(0), "First check");

            Thread.Sleep(50);

            Assert.That(state.Counter, Is.EqualTo(1), "Second check");

            Thread.Sleep(50);

            Assert.That(state.Counter, Is.EqualTo(2), "Third check");

            _threadFactory.Dispose();

            Thread.Sleep(50);

            Assert.That(state.Counter, Is.EqualTo(2), "Last check");
        }

        [Test]
        public void DelayedExecution()
        {
            StateObject state = new StateObject();

            _threadFactory.ScheduleExecution(SimpleCallback, state, 100, Timeout.Infinite);

            Thread.Sleep(75);

            Assert.That(state.Counter, Is.EqualTo(0), "First check");

            Thread.Sleep(50);

            Assert.That(state.Counter, Is.EqualTo(1), "Second check");

            Thread.Sleep(50);

            Assert.That(state.Counter, Is.EqualTo(1), "Last check");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DelayedExecutionWithException(bool critical)
        {
            StateObject state = new StateObject();

            _threadFactory.ScheduleExecution(ExceptionCallback, state, 10, Timeout.Infinite, critical);

            AwaitLogMessage();

            Assert.That(_message.Item1 == LogLevel.Critical, Is.EqualTo(critical), "Failure received");
            Assert.That(_message.Item1 == LogLevel.Error, Is.EqualTo(!critical), "Warning received");
        }

        private void SimpleCallback(StateObject state)
        {
            state.Counter++;
            _callbackReceivedEvent.Set();
        }

        private void ExceptionCallback(StateObject state)
        {
            if (state.Counter++ < MaxTrows)
            {
                throw new Exception(ExceptionMsg);
            }
        }

        private class StateObject
        {
            public int Counter { get; set; }
        }

        private void AwaitLogMessage()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.ElapsedMilliseconds < 50 && _message == null)
            {
                Thread.Sleep(1);
            }
        }
    }
}
