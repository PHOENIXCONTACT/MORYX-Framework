using System;
using System.Threading;
using Marvin.Modules;
using Marvin.TestTools.UnitTest;
using Marvin.Threading;
using NUnit.Framework;

namespace Marvin.Tests.Threading
{
    [TestFixture]
    public class ParallelOperationTests : IModuleErrorReporting
    {
        private const string ExceptionMsg = "Hello World!";
        private const int MaxTrows = 3;
        private const int SleepTime = 1000;

        private ParallelOperations _threadFactory;
        private readonly ManualResetEventSlim _callbackReceivedEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _failureReceivedEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _warningReceivedEvent = new ManualResetEventSlim(false);
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
            _threadFactory = new ParallelOperations
            {
                FailureReporting = this,
            };

            _callbackReceivedEvent.Reset();
            _failureReceivedEvent.Reset();
            _warningReceivedEvent.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _threadFactory.Dispose();
        }

        [Test]
        public void ExecuteParallel()
        {
            StateObject state = new StateObject();

            _threadFactory.ExecuteParallel(SimpleCallback, state);

            _callbackReceivedEvent.Wait(50);

            Assert.IsTrue(_callbackReceivedEvent.IsSet, "Callback not called.");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ExecuteParallelWithException(bool critical)
        {
            StateObject state = new StateObject();

            _threadFactory.ExecuteParallel(ExceptionCallback, state, critical);

            if (critical)
            {
                _failureReceivedEvent.Wait(100);
            }
            else
            {
                _warningReceivedEvent.Wait(100);
            }

            Assert.AreEqual(critical, _failureReceivedEvent.IsSet, "Failure received");
            Assert.AreEqual(!critical, _warningReceivedEvent.IsSet, "Warning received");
        }

        
        [Test]
        public void ScheduleExecutionWithStop()
        {
            StateObject state = new StateObject();

            int id = _threadFactory.ScheduleExecution(SimpleCallback, state, 100, 50);

            Thread.Sleep(75);

            Assert.AreEqual(0, state.Counter, "First check");

            Thread.Sleep(50);

            Assert.AreEqual(1, state.Counter, "Second check");

            Thread.Sleep(50);

            Assert.AreEqual(2, state.Counter, "Third check");

            _threadFactory.StopExecution(id);

            Thread.Sleep(50);

            Assert.AreEqual(2, state.Counter, "Last check");
        }

        [Test]
        public void ScheduleExecutionWithWrongStop()
        {
            StateObject state = new StateObject();

            int id = _threadFactory.ScheduleExecution(SimpleCallback, state, 200, 100);

            Thread.Sleep(150);

            Assert.AreEqual(0, state.Counter, "First check");

            Thread.Sleep(100);

            Assert.AreEqual(1, state.Counter, "Second check");

            Thread.Sleep(100);

            Assert.AreEqual(2, state.Counter, "Third check");

            _threadFactory.StopExecution(42);

            Thread.Sleep(100);

            Assert.AreEqual(3, state.Counter, "Last check");
        }

        [Test]
        public void ScheduleExecutionWithDispose()
        {
            StateObject state = new StateObject();

            _threadFactory.ScheduleExecution(SimpleCallback, state, 100, 50);

            Thread.Sleep(75);

            Assert.AreEqual(0, state.Counter, "First check");

            Thread.Sleep(50);

            Assert.AreEqual(1, state.Counter, "Second check");

            Thread.Sleep(50);

            Assert.AreEqual(2, state.Counter, "Third check");

            _threadFactory.Dispose();

            Thread.Sleep(50);

            Assert.AreEqual(2, state.Counter, "Last check");
        }

        [Test]
        public void DelayedExecution()
        {
            StateObject state = new StateObject();

            _threadFactory.ScheduleExecution(SimpleCallback, state, 100, Timeout.Infinite);

            Thread.Sleep(75);

            Assert.AreEqual(0, state.Counter, "First check");

            Thread.Sleep(50);

            Assert.AreEqual(1, state.Counter, "Second check");

            Thread.Sleep(50);

            Assert.AreEqual(1, state.Counter, "Last check");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DelayedExecutionWithException(bool critical)
        {
            StateObject state = new StateObject();

            _threadFactory.ScheduleExecution(ExceptionCallback, state, 10, Timeout.Infinite, critical);

            if (critical)
            {
                _failureReceivedEvent.Wait(50);
            }
            else
            {
                _warningReceivedEvent.Wait(50);
            }

            Assert.AreEqual(critical, _failureReceivedEvent.IsSet, "Failure received");
            Assert.AreEqual(!critical, _warningReceivedEvent.IsSet, "Warning received");
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

        private void ExceptionCallback()
        {
            throw new Exception(ExceptionMsg);
        }

        private void ExceptionCallback(StateObject state)
        {
            if (state.Counter++ < MaxTrows)
            {
                throw new Exception(ExceptionMsg);
            }
        }

        public void ReportFailure(object sender, Exception exception)
        {
            _failureReceivedEvent.Set();
        }

        public void ReportWarning(object sender, Exception exception)
        {
            _warningReceivedEvent.Set();
        }

        private class StateObject
        {
            public int Counter { get; set; }
        }
    }
}