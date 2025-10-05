// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;
using Moryx.Runtime.Tests.Mocks;
using Moryx.Runtime.Tests.Modules;
using Moq;
using NUnit.Framework;
using Moryx.Configuration;

namespace Moryx.Runtime.Tests
{
    [TestFixture]
    public class ModuleBaseTest
    {
        private TestModule _moduleUnderTest;

        [SetUp]
        public void Init()
        {
            var configManagerMock = new Mock<IConfigManager>();
            configManagerMock.Setup(c => c.GetConfiguration(typeof(TestConfig), It.IsAny<string>(), false)).Returns(new TestConfig
            {
                Strategy = new StrategyConfig { PluginName = "Test" },
                StrategyName = "Test"
            });


            _moduleUnderTest = new TestModule(null, configManagerMock.Object, new TestLoggerMgmt());
        }

        [Test]
        public void StrategiesInConfigFound()
        {
            var casted = (IServerModule) _moduleUnderTest;
            casted.Initialize();

            var containerConfig = _moduleUnderTest.Strategies;

            Assert.That(containerConfig.Count, Is.GreaterThanOrEqualTo(1), "No strategy found!");
            Assert.That(containerConfig.ContainsKey(typeof(IStrategy)), "Wrong type!");
            Assert.That(containerConfig[typeof(IStrategy)], Is.EqualTo("Test"), "Wrong implementation instance set!");
        }
    }
}
