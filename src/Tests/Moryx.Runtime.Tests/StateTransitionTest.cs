// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Kernel;
using Moryx.Runtime.Modules;
using Moryx.Runtime.Tests.Mocks;
using Moryx.Runtime.Tests.Modules;
using Moq;
using NUnit.Framework;
using Moryx.Configuration;

namespace Moryx.Runtime.Tests
{
    [TestFixture]
    public class StateTransitionTest
    {
        private TestModule _moduleUnderTest;
        private Mock<IConfigManager> _configManagerMock;

        [SetUp]
        public void Setup()
        {
            _configManagerMock = new Mock<IConfigManager>();
            _configManagerMock.Setup(c => c.GetConfiguration(typeof(TestConfig), It.IsAny<string>(), false)).Returns(new TestConfig
            {
                Strategy = new StrategyConfig()
            });

            _moduleUnderTest = new TestModule(new ModuleContainerFactory(), _configManagerMock.Object, new TestLoggerMgmt());
        }

        [Test]
        public void StoppedToReady()
        {
            var casted = (IServerModule) _moduleUnderTest;
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Stopped), "Module not in stopped state!");

            // Call initialize
            casted.Initialize();

            // Validate result
            Assert.That(_moduleUnderTest.LastInvoke, Is.EqualTo(InvokedMethod.Initialize), "Initialize was not called!");
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Ready), "Module did not enter ready state!");
        }

        [Test]
        public void ReadyToRunning()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Ready), "Module not in ready state!");

            // Call initialize
            casted.Start();

            // Validate result
            Assert.That(_moduleUnderTest.LastInvoke, Is.EqualTo(InvokedMethod.Start), "Start was not called!");
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Running), "Module did not enter running state!");
        }

        [Test]
        public void RunningToStopped()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            casted.Start();
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Running), "Module not in running state!");

            // Call initialize
            casted.Stop();

            // Validate result
            Assert.That(_moduleUnderTest.LastInvoke, Is.EqualTo(InvokedMethod.Stop), "Stop was not called!");
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Stopped), "Module did not enter stopped state!");
        }

        [Test]
        public void InitializeFails()
        {
            _moduleUnderTest.CurrentMode = TestMode.MoryxException;
            var casted = (IServerModule)_moduleUnderTest;
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Stopped), "Module not in stopped state!");

            // Call initialize
            casted.Initialize();

            // Validate result
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Failure), "Module did not detect error!");
        }

        [Test]
        public void StartFails()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            _moduleUnderTest.CurrentMode = TestMode.MoryxException;
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Ready), "Module not in ready state!");

            // Call initialize
            casted.Start();

            // Validate result
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Failure), "Module did not detect error!");
        }

        [Test]
        public void StopFails()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            casted.Start();
            _moduleUnderTest.CurrentMode = TestMode.MoryxException;
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Running), "Module not in running state!");

            // Call initialize
            casted.Stop();

            // Validate result
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Stopped), "Module was not stopped!");
        }

        [Test]
        public void FailureInStopped()
        {
            var module = new DelayedExceptionModule(new ModuleContainerFactory(), _configManagerMock.Object, new TestLoggerMgmt());
            var casted = (IServerModule) module;

            casted.Initialize();
            casted.Start();

            casted.Stop();
            module.WaitEvent.Set();
        }
    }
}
