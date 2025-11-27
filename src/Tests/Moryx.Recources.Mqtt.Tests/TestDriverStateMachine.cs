// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.TestTools;
using Moryx.Drivers.Mqtt;
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
    public class TestDriverStateMachine(MqttProtocolVersion version)
    {
        private Mock<IMqttClient> _mockClient;
        private MqttDriver _driver;

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
                MqttVersion = version
            };

            //Setup mock for MQTT-Client
            _mockClient = new Mock<IMqttClient>();
            var options = new MqttClientOptionsBuilder()
                .WithClientId(_driver.Id.ToString())
                .WithTcpServer(_driver.BrokerUrl, _driver.Port)
                .Build();
            _mockClient.Setup(m => m.ConnectAsync(It.IsAny<MqttClientOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MqttClientConnectResult(), TimeSpan.FromMilliseconds(100));
        }

        [Test(Description = $"After stopping the driver it should be Offline")]
        public async Task Stop_Always_EndsInDisconnectedState()
        {
            //Arrange
            _driver.InitializeForTest(_mockClient.Object);
            await ((IAsyncPlugin)_driver).StartAsync();

            //Act
            await ((IAsyncPlugin)_driver).StopAsync();

            //Assert I
            Assert.That(_driver.CurrentState.Classification, Is.EqualTo(StateClassification.Offline));
        }

        [Test(Description = $"After restarting the driver it should be Initializing")]
        public async Task Start_AfterStop_LeadsToConnectingToBrokerState()
        {
            //Arrange
            _driver.InitializeForTest(_mockClient.Object);
            await ((IAsyncPlugin)_driver).StartAsync();

            //Act
            await ((IAsyncPlugin)_driver).StopAsync();
            await ((IAsyncPlugin)_driver).StartAsync();

            //Assert I
            Assert.That(_driver.CurrentState.Classification, Is.EqualTo(StateClassification.Initializing));
        }
    }
}

