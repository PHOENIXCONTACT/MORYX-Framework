using System;
using System.Linq;
using System.Threading;
using Marvin.Logging;
using Marvin.TestTools.UnitTest;
using Marvin.Tools;
using NUnit.Framework;

namespace Marvin.PlatformTools.Tests.Tools
{
    [TestFixture]
    public class WorkerThreadTests
    {
        private const string ExceptionMessage = "Some exception...";

        private WorkerThread _thread;
        private bool _throw;

        private readonly ManualResetEventSlim _workerEvent = new ManualResetEventSlim(false);
        private int _workerCount;
        private DummyLogger _logger;

        [SetUp]
        public void Setup()
        {
            _throw = false;
            _workerEvent.Reset();
            _workerCount = 0;
            _logger = new DummyLogger();
            _thread = new WorkerThread("TestThread", DoSomething, 100, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _thread.Dispose();
        }

        private void DoSomething()
        {
            _workerCount++;
            _workerEvent.Set();

            if (_throw)
            {
                throw new Exception(ExceptionMessage);
            }
        }

        [Test]
        public void ConstructorWithoutDelegate()
        {
            Assert.Throws<ArgumentNullException>(() => new WorkerThread("TestThread", null, 100, _logger));
        }

        [TestCase(1)]
        public void ConstructorTest(int sleepTime)
        {
            Assert.DoesNotThrow(delegate
            {
                new WorkerThread("TestThread", DoSomething, sleepTime, _logger);
            });
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void TestConstructorWithIllegalSleepTime(int sleepTime)
        {
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                new WorkerThread("TestThread", DoSomething, sleepTime, _logger);
            });
        }

        [Test]
        public void TestLiveCycle()
        {
            Assert.NotNull(_thread.Token, "CancellationToken");

            _thread.Join(100); // Ntohing bad should happen...

            Assert.IsFalse(_thread.IsAlive, "Thread marked as already alive.");

            _workerEvent.Wait(200);

            Assert.IsFalse(_workerEvent.IsSet, "Thread is already started.");

            _thread.Start();

            Assert.IsTrue(_thread.IsAlive, "Thread not marked as alive.");

            _workerEvent.Wait(200);

            Assert.NotNull(_logger.Messages.FirstOrDefault(m => m.Message.Contains("Starting thread")), "Start message not found.");
            Assert.IsTrue(_workerEvent.IsSet, "Thread is not started.");

            _thread.Stop();

            Assert.IsTrue(_thread.Token.IsCancellationRequested, "IsCancellationRequested not set.");
            Assert.NotNull(_logger.Messages.FirstOrDefault(m => m.Message.Contains("Stopping thread")), "Stopping message not found.");

            _thread.Join(100);

            Assert.IsFalse(_thread.IsAlive, "Thread marked as still alive.");
            Assert.NotNull(_logger.Messages.FirstOrDefault(m => m.Message.Contains("finished")), "Finished message not found.");

            _workerEvent.Reset();
            _workerEvent.Wait(200);

            Assert.IsFalse(_workerEvent.IsSet, "Thread is still running.");
            Assert.Null(_logger.Messages.FirstOrDefault(m => m.Exception != null), "Exceptions caught");
        }

        [Test]
        public void StartDisposed()
        {
            _thread.Dispose();
        }

        [Test]
        public void StartTwice()
        {
            _thread.Start();

            Assert.IsTrue(_thread.IsAlive, "Thread not marked as alive.");
            Assert.Throws<InvalidOperationException>(() => _thread.Start());
        }

        [Test]
        public void TestRestart()
        {
            _thread.Start();
            Assert.IsTrue(_thread.IsAlive, "Thread not marked as alive.");

            _thread.Stop();
            Assert.IsTrue(_thread.Token.IsCancellationRequested, "IsCancellationRequested not set.");

            _thread.Join(100);

            Assert.IsFalse(_thread.IsAlive, "Thread marked as still alive.");
            Assert.Throws<InvalidOperationException>(() => _thread.Start());
        }

        [Test]
        public void TestDispose()
        {
            _thread.Start();

            Assert.IsTrue(_thread.IsAlive, "Thread not marked as alive.");

            _thread.Dispose();

            Assert.IsTrue(_thread.Token.IsCancellationRequested, "IsCancellationRequested not set.");

            _thread.Join(100);

            Assert.IsFalse(_thread.IsAlive, "Thread marked as still alive.");
        }

        [Test]
        public void TestCatchException()
        {
            _throw = true;

            _thread.Start();

            Assert.IsTrue(_thread.IsAlive, "Thread not marked as alive.");

            _workerEvent.Wait(200);

            Assert.IsTrue(_workerEvent.IsSet, "Thread is not working.");
            Assert.AreEqual(1, _workerCount, "Counter not increased");

            _workerEvent.Reset();
            _workerEvent.Wait(200);

            Assert.IsTrue(_workerEvent.IsSet, "Thread is not working anymore.");
            Assert.AreEqual(2, _workerCount, "Counter not increased");

            Assert.AreEqual(2, _logger.Messages.Count(m => m.Exception != null && m.Exception.Message == ExceptionMessage && m.Level == LogLevel.Error), "Exceptions caught");
        }
    }
}