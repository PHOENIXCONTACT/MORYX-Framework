// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Threading;
using Marvin.TestTools.UnitTest;
using NUnit.Framework;

namespace Marvin.Tests.Threading
{
    [TestFixture]
    public class NotSoParallelOpsTests
    {
        private const string ExceptionMsg = "Hello World!";
        private const int MaxTrows = 3;
        private const int SleepTime = 5000;

        private NotSoParallelOps _threadFactory;
        private readonly ManualResetEventSlim _callbackReceivedEvent = new ManualResetEventSlim(false);
        private DummyLogger _logger;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new DummyLogger();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }

        [SetUp]
        public void Setup()
        {
            _logger.ClearBuffer();
            _threadFactory = new NotSoParallelOps();

            _callbackReceivedEvent.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _threadFactory.Dispose();
        }

        [Test]
        public void ExecuteParallel()
        {
            // Arrange
            var state = new StateObject();

            // Act
            _threadFactory.ExecuteParallel(SimpleCallback, state);

            // Assert
            Assert.IsTrue(_callbackReceivedEvent.IsSet, "Callback not called.");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ScheduledExecuteParallelWithException(bool critical)
        {
            // Arrange
            var state = new StateObject();

            // Act
            _threadFactory.ScheduleExecution(ExceptionCallback, state, 10, Timeout.Infinite, critical);

            // Assert
            Assert.AreEqual(true, _threadFactory.WaitForScheduledExecution(5000));
            Assert.AreEqual(1, _threadFactory.ScheduledExecutionExceptions().Count());
        }


        [Test]
        public void ScheduleExecutionWithStop()
        {
            // Arrange
            var state = new StateObject();

            // Act
            var id = _threadFactory.ScheduleExecution(SimpleCallback, state, 100, 50);

            Thread.Sleep(75);

            Assert.AreEqual(0, state.Counter, "First check");

            Thread.Sleep(50);

            Assert.AreEqual(1, state.Counter, "Second check");

            Thread.Sleep(50);

            Assert.AreEqual(2, state.Counter, "Third check");

            _threadFactory.StopExecution(id);

            // Assert
            Assert.AreEqual(true, _threadFactory.WaitForScheduledExecution(5000));
            Assert.AreEqual(2, state.Counter, "Last check");
        }

        [Test]
        public void ScheduleExecutionWithWrongStop()
        {
            // Arrange
            var state = new StateObject();

            // Act
            _threadFactory.ScheduleExecution(SimpleCallback, state, 200, 100);

            Thread.Sleep(150);

            Assert.AreEqual(0, state.Counter, "First check");

            Thread.Sleep(100);

            Assert.AreEqual(1, state.Counter, "Second check");

            Thread.Sleep(100);

            Assert.AreEqual(2, state.Counter, "Third check");

            _threadFactory.StopExecution(42);

            Thread.Sleep(100);

            // Assert
            Assert.AreEqual(3, state.Counter, "Last check");
            Assert.AreEqual(false, _threadFactory.WaitForScheduledExecution(500));
        }

        [Test]
        public void ScheduleExecutionWithDispose()
        {
            // Arrange
            var state = new StateObject();

            // Act
            _threadFactory.ScheduleExecution(SimpleCallback, state, 100, 50);

            Thread.Sleep(75);

            Assert.AreEqual(0, state.Counter, "First check");

            Thread.Sleep(50);

            Assert.AreEqual(1, state.Counter, "Second check");

            Thread.Sleep(50);

            Assert.AreEqual(2, state.Counter, "Third check");

            _threadFactory.Dispose();

            // Assert
            Assert.AreEqual(true, _threadFactory.WaitForScheduledExecution(5000));
            Assert.AreEqual(2, state.Counter, "Last check");
        }

        [Test]
        public void DelayedExecution()
        {
            // Arrange
            var state = new StateObject();

            // Act
            _threadFactory.ScheduleExecution(SimpleCallback, state, 100, Timeout.Infinite);

            Thread.Sleep(75);

            Assert.AreEqual(0, state.Counter, "First check");

            Thread.Sleep(50);

            // Assert
            Assert.AreEqual(1, state.Counter, "Second check");
            Assert.AreEqual(true, _threadFactory.WaitForScheduledExecution(5000));
            Assert.AreEqual(1, state.Counter, "Last check");
        }

        [Test]
        public void DelayedExecutionRunsInTimeout()
        {
            // Arrange
            var state = new StateObject();

            // Act
            _threadFactory.ScheduleExecution(SleepingCallback, state, 100, Timeout.Infinite);

            // Assert
            Assert.AreEqual(false, _threadFactory.WaitForScheduledExecution(500));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DelayedExecutionWithException(bool critical)
        {
            // Arrange
            var state = new StateObject();

            // Act
            _threadFactory.ScheduleExecution(ExceptionCallback, state, 10, Timeout.Infinite, critical);

            // Assert
            Assert.AreEqual(true, _threadFactory.WaitForScheduledExecution(5000));
            Assert.AreEqual(1, _threadFactory.ScheduledExecutionExceptions().Count());
        }

        private void SimpleCallback(StateObject state)
        {
            state.Counter++;
            _callbackReceivedEvent.Set();
        }

        private void SleepingCallback(StateObject state)
        {
            Thread.Sleep(SleepTime);

            state.Counter++;
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
    }
}
