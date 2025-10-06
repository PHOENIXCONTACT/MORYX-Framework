// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.TestTools;
using Moryx.Drivers.Mqtt;
using Moryx.Drivers.Mqtt.DriverStates;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Tools;
using MQTTnet.Client;
using MQTTnet.Formatter;
using NUnit.Framework;

namespace Moryx.Resources.Mqtt.Tests
{
    [TestFixture(MqttProtocolVersion.V310)]
    [TestFixture(MqttProtocolVersion.V311)]
    [TestFixture(MqttProtocolVersion.V500)]
    public class TestDriverStateMachine
    {
        private Mock<IMqttClient> _mockClient;
        private MqttDriver _driver;
        private MqttProtocolVersion _version;

        public TestDriverStateMachine(MqttProtocolVersion version) => _version = version;

        [SetUp]
        public void Setup()
        {
            ReflectionTool.TestMode = true;

            //Setup MqttDriver I
            _driver = new MqttDriver
            {
                Identifier = "topicDriver",
                Id = 4,
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
                Channels = new ReferenceCollectionMock<MqttTopic>(),
                MqttVersion = _version
            };

            //Setup mock for MQTT-Client
            _mockClient = new Mock<IMqttClient>();
            var options = new MqttClientOptionsBuilder()
                .WithClientId(_driver.Id.ToString())
                .WithTcpServer(_driver.BrokerURL, _driver.Port)
                .Build();
            _mockClient.Setup(m => m.ConnectAsync(It.IsAny<MqttClientOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MqttClientConnectResult(), TimeSpan.FromMilliseconds(100));
        }

        [Test(Description = $"After stopping the driver it should be in the {nameof(DisconnectedState)}")]
        public void Stop_Always_EndsInDisconnectedState()
        {
            //Arrange
            _driver.InitializeForTest(_mockClient.Object);
            ((IPlugin)_driver).Start();

            //Act
            ((IPlugin)_driver).Stop();

            //Assert I
            Assert.That(_driver.State, Is.InstanceOf(typeof(DisconnectedState)));
        }

        [Test(Description = $"After restarting the driver it should be in the {nameof(ConnectingToBrokerState)}")]
        public void Start_AfterStop_LeadsToConnectingToBrokerState()
        {
            //Arrange
            _driver.InitializeForTest(_mockClient.Object);
            ((IPlugin)_driver).Start();

            //Act
            ((IPlugin)_driver).Stop();
            ((IPlugin)_driver).Start();

            //Assert I
            Assert.That(_driver.State, Is.InstanceOf(typeof(ConnectingToBrokerState)));
        }
    }
}

