// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
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
        public async Task StoppedToReady()
        {
            var casted = (IServerModule)_moduleUnderTest;
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Stopped), "Module not in stopped state!");

            // Call initialize
            await casted.Initialize();

            // Validate result
            Assert.That(_moduleUnderTest.LastInvoke, Is.EqualTo(InvokedMethod.Initialize), "Initialize was not called!");
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Ready), "Module did not enter ready state!");
        }

        [Test]
        public async Task ReadyToRunning()
        {
            var casted = (IServerModule)_moduleUnderTest;
            await casted.Initialize();
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Ready), "Module not in ready state!");

            // Call initialize
            await casted.StartAsync();

            // Validate result
            Assert.That(_moduleUnderTest.LastInvoke, Is.EqualTo(InvokedMethod.Start), "Start was not called!");
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Running), "Module did not enter running state!");
        }

        [Test]
        public async Task RunningToStopped()
        {
            var casted = (IServerModule)_moduleUnderTest;
            await casted.Initialize();
            await casted.StartAsync();
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Running), "Module not in running state!");

            // Call initialize
            await casted.StopAsync();

            // Validate result
            Assert.That(_moduleUnderTest.LastInvoke, Is.EqualTo(InvokedMethod.Stop), "Stop was not called!");
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Stopped), "Module did not enter stopped state!");
        }

        [Test]
        public async Task InitializeFails()
        {
            _moduleUnderTest.CurrentMode = TestMode.MoryxException;
            var casted = (IServerModule)_moduleUnderTest;
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Stopped), "Module not in stopped state!");

            // Call initialize
            await casted.Initialize();

            // Validate result
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Failure), "Module did not detect error!");
        }

        [Test]
        public async Task StartFails()
        {
            var casted = (IServerModule)_moduleUnderTest;
            await casted.Initialize();
            _moduleUnderTest.CurrentMode = TestMode.MoryxException;
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Ready), "Module not in ready state!");

            // Call initialize
            await casted.StartAsync();

            // Validate result
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Failure), "Module did not detect error!");
        }

        [Test]
        public async Task StopFails()
        {
            var casted = (IServerModule)_moduleUnderTest;
            await casted.Initialize();
            await casted.StartAsync();
            _moduleUnderTest.CurrentMode = TestMode.MoryxException;
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Running), "Module not in running state!");

            // Call initialize
            await casted.StopAsync();

            // Validate result
            Assert.That(casted.State, Is.EqualTo(ServerModuleState.Stopped), "Module was not stopped!");
        }

        [Test]
        public async Task FailureInStopped()
        {
            var module = new DelayedExceptionModule(new ModuleContainerFactory(), _configManagerMock.Object, new TestLoggerMgmt());
            var casted = (IServerModule)module;

            await casted.Initialize();
            await casted.StartAsync();

            await casted.StopAsync();
            module.WaitEvent.Set();
        }
    }
}
