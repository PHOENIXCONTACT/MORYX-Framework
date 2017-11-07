using System.Linq;
using System.Threading;
using Marvin.Runtime.Base.Tests.Mocks;
using Marvin.Runtime.Base.Tests.Modules;
using NUnit.Framework;

namespace Marvin.Runtime.Base.Tests
{
    [TestFixture]
    public class StateTransitionTest
    {
        private TestModule _moduleUnderTest;

        [SetUp]
        public void Init()
        {
            _moduleUnderTest = new TestModule
            {
                ConfigManager = new TestConfigManager(),
                LoggerManagement = new TestLoggerMgmt()
            };
        }

        [Test]
        public void StoppedToReady()
        {
            var casted = (IServerModule) _moduleUnderTest;
            Assert.AreEqual(ServerModuleState.Stopped, casted.State.Current, "Module not in stopped state!");

            // Call initialize
            casted.Initialize();

            // Validate result
            Assert.AreEqual(InvokedMethod.Initialize, _moduleUnderTest.LastInvoke, "Initialize was not called!");
            Assert.AreEqual(ServerModuleState.Ready, casted.State.Current, "Module did not enter ready state!");
        }

        [Test]
        public void ReadyToRunning()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            Assert.AreEqual(ServerModuleState.Ready, casted.State.Current, "Module not in ready state!");

            // Call initialize
            casted.Start();

            // Validate result
            Assert.AreEqual(InvokedMethod.Start, _moduleUnderTest.LastInvoke, "Start was not called!");
            Assert.AreEqual(ServerModuleState.Running, casted.State.Current, "Module did not enter running state!");
        }

        [Test]
        public void RunningToStopped()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            casted.Start();
            Assert.AreEqual(ServerModuleState.Running, casted.State.Current, "Module not in running state!");

            // Call initialize
            casted.Stop();

            // Validate result
            Assert.AreEqual(InvokedMethod.Stop, _moduleUnderTest.LastInvoke, "Stop was not called!");
            Assert.AreEqual(ServerModuleState.Stopped, casted.State.Current, "Module did not enter stopped state!");
        }

        [Test]
        public void InitializeFails()
        {
            _moduleUnderTest.CurrentMode = TestMode.MarvinException;
            var casted = (IServerModule)_moduleUnderTest;
            Assert.AreEqual(ServerModuleState.Stopped, casted.State.Current, "Module not in stopped state!");

            // Call initialize
            casted.Initialize();

            // Validate result
            Assert.AreEqual(ServerModuleState.Failure, casted.State.Current, "Module did not detect error!");
        }

        [Test]
        public void StartFails()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            _moduleUnderTest.CurrentMode = TestMode.MarvinException;
            Assert.AreEqual(ServerModuleState.Ready, casted.State.Current, "Module not in ready state!");

            // Call initialize
            casted.Start();

            // Validate result
            Assert.AreEqual(ServerModuleState.Failure, casted.State.Current, "Module did not detect error!");
        }

        [Test]
        public void StopFails()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            casted.Start();
            _moduleUnderTest.CurrentMode = TestMode.MarvinException;
            Assert.AreEqual(ServerModuleState.Running, casted.State.Current, "Module not in running state!");

            // Call initialize
            casted.Stop();

            // Validate result
            Assert.AreEqual(ServerModuleState.Failure, casted.State.Current, "Module did not detect error!");
        }

        [Test]
        public void RunningToWarning()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            casted.Start();
            Assert.AreEqual(ServerModuleState.Running, casted.State.Current, "Module not in running state!");

            _moduleUnderTest.RaiseException(false);
            Thread.Sleep(100);
            Assert.AreEqual(ServerModuleState.Warning, casted.State.Current, "Module did not enter warning state!");
        }

        [Test]
        public void WarningToRunning()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            casted.Start();
            _moduleUnderTest.RaiseException(false);
            Thread.Sleep(200);
            Assert.AreEqual(ServerModuleState.Warning, casted.State.Current, "Module not in warning state!");

            foreach (var notification in casted.Notifications.ToArray())
            {
                notification.Confirm();
            }
            Assert.AreEqual(ServerModuleState.Running, casted.State.Current, "Module did not return to running!");
        }

        [Test]
        public void RunningToFailure()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            casted.Start();
            Assert.AreEqual(ServerModuleState.Running, casted.State.Current, "Module not in running state!");

            _moduleUnderTest.RaiseException(true);
            Thread.Sleep(200);
            Assert.AreEqual(ServerModuleState.Failure, casted.State.Current, "Module did not enter failure state!");
        }

        [Test]
        public void FailureInStopped()
        {
            var module = new DelayedExceptionModule
            {
                ConfigManager = new TestConfigManager(),
                LoggerManagement = new TestLoggerMgmt()
            };
            var casted = (IServerModule) module;

            casted.Initialize();
            casted.Start();

            casted.Stop();
            module.WaitEvent.Set();
        }
    }
}
