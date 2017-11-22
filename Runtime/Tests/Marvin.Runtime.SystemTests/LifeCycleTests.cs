using System;
using System.Threading;
using Marvin.Runtime.Modules;
using Marvin.TestTools.SystemTest;
using NUnit.Framework;

namespace Marvin.Runtime.SystemTests
{
    /// <summary>
    /// These tests shall check two aspects: They shall verify the HoG functionality but also wether the HeartOfGoldController is working as expected.
    /// </summary>
    [TestFixture]
    public class LifeCycleTests : IDisposable
    {
        private HeartOfGoldController _hogController;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            HogHelper.CopyTestModules();

            _hogController = new HeartOfGoldController
            {
                RuntimeDir = HogHelper.RuntimeDir,
                ConfigDir = HogHelper.ConfigDirParam,
                ExecutionTimeout = 120
            };

            Console.WriteLine("Starting HeartOfGold");

            bool started = _hogController.StartHeartOfGold();
            _hogController.CreateClients();

            Assert.IsTrue(started, "Can't start HeartOfGold.");
            Assert.IsFalse(_hogController.Process.HasExited, "HeartOfGold has exited unexpectedly.");
        }

        [SetUp]
        public void SetUp()
        {
            bool result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 10);

            if (! result)
            {
                _hogController.StartService("DependentTestModule");

                result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 5);
            }

            Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Running'");
        }

        /// <summary>
        /// Shut down the system test.
        /// </summary>
        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            if (_hogController.Process != null && !_hogController.Process.HasExited)
            {
                Console.WriteLine("Trying to stop HeartOfGold");

                _hogController.StopHeartOfGold(10);

                if (!_hogController.Process.HasExited)
                {
                    Console.WriteLine("Killing HeartOfGold");
                    _hogController.Process.Kill();

                    Thread.Sleep(1000);

                    Assert.IsTrue(_hogController.Process.HasExited, "Can't kill HeartOfGold.");
                }

                HogHelper.DeleteTestModules();
            }
        }

        public void Dispose()
        {
            if (_hogController != null)
            {
                _hogController.Dispose();
                _hogController = null;
            }
        }

        [Test]
        public void TestStartStopDependentTestService()
        {
            bool result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Initial state of service 'DependentTestModule' is not 'Running' ");

            _hogController.StopService("DependentTestModule");

            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Stopped, 5);
            Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Stopped'");

            _hogController.StartService("DependentTestModule");

            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Running' ");
        }

        [TestCase("TestModule")]
        [TestCase("DependentTestModule")]
        public void TestStartStopTestService(string serviceToStart)
        {
            bool result = _hogController.WaitForService("TestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Initial state of service 'TestModule' is not 'Running' ");

            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Initial state of service 'DependentTestModule' is not 'Running' ");

            _hogController.StopService("TestModule");

            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Stopped, 5);
            Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Stopped'");

            result = _hogController.WaitForService("TestModule", ServerModuleState.Stopped, 5);
            Assert.IsTrue(result, "Service 'TestModule' did not reach state 'Stopped'");

            _hogController.StartService(serviceToStart);

            result = _hogController.WaitForService("TestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service 'TestModule' did not reach state 'Running'");

            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Running' ");
        }

        [Test]
        public void TestReincarnateDependentTestServiceAsync()
        {
            bool result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Initial state of service 'DependentTestModule' is not 'Running' ");

            _hogController.ReincarnateServiceAsync("DependentTestModule");

            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Stopping, 5);
            Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Stopping'");

            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Running' ");
        }

        [Test]
        public void TestReincarnateTestServiceAsync()
        {
            bool result = _hogController.WaitForService("TestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Initial state of service 'TestModule' is not 'Running' ");

            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Initial state of service 'DependentTestModule' is not 'Running' ");

            _hogController.ReincarnateServiceAsync("TestModule");

            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Stopping, 5);
            Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Stopping'");

            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Stopped, 5);
            Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Stopped'");

            result = _hogController.WaitForService("TestModule", ServerModuleState.Stopping, 5);
            Assert.IsTrue(result, "Service 'TestModule' did not reach state 'Stopping'");

            result = _hogController.WaitForService("TestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Initial state of service 'TestModule' is not 'Running' ");

            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Running' ");
        }
    }
}