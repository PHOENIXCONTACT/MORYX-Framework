// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Configuration;
using Moryx.Runtime.Kernel;
using Moryx.Runtime.Modules;
using Moryx.Runtime.Tests.Mocks;
using Moryx.Runtime.Tests.Modules;
using Moq;
using NUnit.Framework;
using Moryx.Configuration;
using System;

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
            Assert.AreEqual(ServerModuleState.Stopped, casted.State, "Module not in stopped state!");

            // Call initialize
            casted.Initialize();

            // Validate result
            Assert.AreEqual(InvokedMethod.Initialize, _moduleUnderTest.LastInvoke, "Initialize was not called!");
            Assert.AreEqual(ServerModuleState.Ready, casted.State, "Module did not enter ready state!");
        }

        [Test]
        public void ReadyToRunning()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            Assert.AreEqual(ServerModuleState.Ready, casted.State, "Module not in ready state!");

            // Call initialize
            casted.Start();

            // Validate result
            Assert.AreEqual(InvokedMethod.Start, _moduleUnderTest.LastInvoke, "Start was not called!");
            Assert.AreEqual(ServerModuleState.Running, casted.State, "Module did not enter running state!");
        }

        [Test]
        public void RunningToStopped()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            casted.Start();
            Assert.AreEqual(ServerModuleState.Running, casted.State, "Module not in running state!");

            // Call initialize
            casted.Stop();

            // Validate result
            Assert.AreEqual(InvokedMethod.Stop, _moduleUnderTest.LastInvoke, "Stop was not called!");
            Assert.AreEqual(ServerModuleState.Stopped, casted.State, "Module did not enter stopped state!");
        }

        [Test]
        public void InitializeFails()
        {
            _moduleUnderTest.CurrentMode = TestMode.MoryxException;
            var casted = (IServerModule)_moduleUnderTest;
            Assert.AreEqual(ServerModuleState.Stopped, casted.State, "Module not in stopped state!");

            // Call initialize
            casted.Initialize();

            // Validate result
            Assert.AreEqual(ServerModuleState.Failure, casted.State, "Module did not detect error!");
        }

        [Test]
        public void StartFails()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            _moduleUnderTest.CurrentMode = TestMode.MoryxException;
            Assert.AreEqual(ServerModuleState.Ready, casted.State, "Module not in ready state!");

            // Call initialize
            casted.Start();

            // Validate result
            Assert.AreEqual(ServerModuleState.Failure, casted.State, "Module did not detect error!");
        }

        [Test]
        public void StopFails()
        {
            var casted = (IServerModule)_moduleUnderTest;
            casted.Initialize();
            casted.Start();
            _moduleUnderTest.CurrentMode = TestMode.MoryxException;
            Assert.AreEqual(ServerModuleState.Running, casted.State, "Module not in running state!");

            // Call initialize
            casted.Stop();

            // Validate result
            Assert.AreEqual(ServerModuleState.Stopped, casted.State, "Module was not stopped!");
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
