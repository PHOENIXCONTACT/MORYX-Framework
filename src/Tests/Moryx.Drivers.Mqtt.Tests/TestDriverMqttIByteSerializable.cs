// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Buffers;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.TestTools;
using Moryx.Drivers.Mqtt.MqttTopics;
using Moryx.Drivers.Mqtt.Tests.TestMessages;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Tools;
using MQTTnet;
using MQTTnet.Formatter;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using NUnit.Framework;

namespace Moryx.Drivers.Mqtt.Tests
{
    [TestFixture(MqttProtocolVersion.V310)]
    [TestFixture(MqttProtocolVersion.V311)]
    [TestFixture(MqttProtocolVersion.V500)]
    public class TestDriverMqttIByteSerializable
    {
        private const int TIMEOUT = 2;
        private const bool MESSAGE_VALUE = true;
        private Mock<IMqttClient> _mockClient;
        private MqttDriver _driver;
        private MqttTopic _topicBoolMqtt;
        private MqttTopic _topicBoolIByteSerializable;
        private MqttProtocolVersion _version;

        public TestDriverMqttIByteSerializable(MqttProtocolVersion version) => _version = version;

        [SetUp]
        public void Setup()
        {
            ReflectionTool.TestMode = true;

            _topicBoolMqtt = new MqttTopicIByteSerializable
            {
                Identifier = "test",
                MessageName = nameof(BoolMqttMessage)
            };

            _topicBoolIByteSerializable = new MqttTopicIByteSerializable
            {
                Identifier = "BoolIByteSerializable",
                MessageName = nameof(BoolByteSerializableMessage)
            };

            ((IInitializable)_topicBoolMqtt).Initialize();
            ((IInitializable)_topicBoolIByteSerializable).Initialize();

            _driver = new MqttDriver
            {
                Identifier = "topicDriver/",
                Id = 4,
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
                Channels = new ReferenceCollectionMock<MqttTopic> { _topicBoolMqtt, _topicBoolIByteSerializable },
                MqttVersion = _version,
                BrokerURL = "mock"
            };

            _mockClient = new Mock<IMqttClient>();
            _mockClient.Setup(m => m.ConnectAsync(It.Is(CorrectClientOptions()), CancellationToken.None))
                .ReturnsAsync(new MqttClientConnectResult());
            _mockClient.Setup(m => m.SubscribeAsync(It.IsAny<MqttClientSubscribeOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MqttClientSubscribeResult(0, Array.Empty<MqttClientSubscribeResultItem>(), "", Array.Empty<MqttUserProperty>()));

            _driver.InitializeForTest(_mockClient.Object);
            ((IPlugin)_driver).Start();
            _driver.OnConnected(new MqttClientConnectedEventArgs(new MqttClientConnectResult())).Wait();
            _topicBoolMqtt.Parent = _driver;
            _topicBoolIByteSerializable.Parent = _driver;

        }

        private Expression<Func<MqttClientOptions, bool>> CorrectClientOptions()
        {
            return o => o.ProtocolVersion == _driver.MqttVersion && o.CleanSession == !_driver.ReconnectWithoutCleanSession
                        && o.ClientId == $"{System.Net.Dns.GetHostName()}-{_driver.Id}-{_driver.Name}";
        }

        [Test(Description = "Publish Message using Driver")]
        public void Send_UsingDriver_IIdentifierMessage_Topic_QOS_Message()
        {
            //Arrange
            _mockClient.Setup(m => m.PublishAsync(
                    It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>()))
                .Callback<MqttApplicationMessage, CancellationToken>((sentMsg, token) => SendMessageBoolMqttMessage(sentMsg, token));
            //Act
            _driver.Send(new BoolMqttMessage { Message = MESSAGE_VALUE, Identifier = _topicBoolMqtt.Identifier });

            //Assert 1
            _mockClient.Verify((m => m.PublishAsync(
                It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>())));
        }

        [Test(Description = "Publish Message using the MqttTopic")]
        public void Send_UsingTopic_IIdentifierMessage_Topic_QOS_Message()
        {
            //Arrange
            _mockClient.Setup(m => m.PublishAsync(
                    It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>()))
                .Callback<MqttApplicationMessage, CancellationToken>((sentMsg, token)
                    => SendMessageBoolMqttMessage(sentMsg, token));
            //Act
            _topicBoolMqtt.Send(new BoolMqttMessage { Message = MESSAGE_VALUE, Identifier = _topicBoolMqtt.Identifier });

            //Assert 1
            _mockClient.Verify((m => m.PublishAsync(
                It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>())));

        }

        private void SendMessageBoolMqttMessage(MqttApplicationMessage mqttMsg, CancellationToken token)
        {
            //Assert 2
            Assert.That(token, Is.EqualTo(CancellationToken.None));
            Assert.That(mqttMsg.Topic.Equals(_driver.Identifier + _topicBoolMqtt.Identifier),
                "Topic should be " + _driver.Identifier + _topicBoolMqtt.Identifier + ", but is " + mqttMsg.Topic);
            Assert.That(mqttMsg.QualityOfServiceLevel, Is.EqualTo(MqttQualityOfServiceLevel.ExactlyOnce),
                "Qos should be ExactlyOnce, but is " + mqttMsg.QualityOfServiceLevel);
            var msg = new BoolMqttMessage();
            msg.FromBytes(mqttMsg.Payload.ToArray());
            Assert.That(msg.Message == MESSAGE_VALUE, "Message should be " + MESSAGE_VALUE + ", but is " + msg.Message);
        }

        [Test(Description = "Publish Message using the MqttTopic and not IIdentifierMessage")]
        public void Send_UsingTopic_IByteSerializableMessage_Topic_QOS_Message()
        {
            //Arrange
            _mockClient.Setup(m => m.PublishAsync(
                    It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>()))
                .Callback<MqttApplicationMessage, CancellationToken>((sentMsg, token)
                    => SendMessageBoolIByteSerializableMessage(sentMsg, token));

            //Act
            _topicBoolIByteSerializable.Send(new BoolByteSerializableMessage { Message = MESSAGE_VALUE });

            //Assert 1
            _mockClient.Verify((m => m.PublishAsync(
                It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>())));

        }

        private void SendMessageBoolIByteSerializableMessage(MqttApplicationMessage sentMsg, CancellationToken token)
        {
            //Assert 2
            Assert.That(token, Is.EqualTo(CancellationToken.None));
            Assert.That(sentMsg.Topic.Equals(_driver.Identifier + _topicBoolIByteSerializable.Identifier),
                "Topic should be " + _driver.Identifier + _topicBoolIByteSerializable.Identifier + ", but is " + sentMsg.Topic);
            Assert.That(sentMsg.QualityOfServiceLevel, Is.EqualTo(MqttQualityOfServiceLevel.ExactlyOnce),
                "Qos should be ExactlyOnce, but is " + sentMsg.QualityOfServiceLevel);
            var msg = new BoolByteSerializableMessage();
            msg.FromBytes(sentMsg.Payload.ToArray());
            Assert.That(msg.Message == MESSAGE_VALUE, "Message should be " + MESSAGE_VALUE + ", but is " + msg.Message);
        }

        [Test(Description = "Receive Message using the Driver")]
        public void Receive_SubscribedTopic_IIdentifierMessage_MessageType_Topic_Message()
        {
            //Arrange
            var driverReceivedMessage = new AutoResetEvent(false);
            var topicReceivedMessage = new AutoResetEvent(false);
            _driver.Received += OnReceivedIIdentifierMessage;
            _driver.Received += (sender, eventArgs) => { driverReceivedMessage.Set(); };
            _topicBoolMqtt.Received += (sender, eventArgs) => { topicReceivedMessage.Set(); };

            //Act
            _driver.Receive(
                new MqttApplicationMessage()
                {
                    Topic = _driver.Identifier + _topicBoolMqtt.Identifier,
                    PayloadSegment = new BoolMqttMessage
                    {
                        Message = MESSAGE_VALUE,
                        Identifier = _topicBoolMqtt.Identifier
                    }.ToBytes()
                });

            //Assert 1
            Assert.That(driverReceivedMessage.WaitOne(TimeSpan.FromSeconds(TIMEOUT)), "Received Event on driver was not raised");
            Assert.That(topicReceivedMessage.WaitOne(TimeSpan.FromSeconds(TIMEOUT)), "Received Event on topic was not raised");
        }
        private void OnReceivedIIdentifierMessage(object sender, object e)
        {
            //Assert 2
            var msg = e as BoolMqttMessage;
            Assert.That(msg != null, "Not the right type: Should be TestMessage but is " + e.GetType());
            Assert.That(msg.Message == MESSAGE_VALUE, "Message should " + MESSAGE_VALUE + ", but is " + msg.Message);
            Assert.That(msg.Identifier.Equals(_topicBoolMqtt.Identifier), "Topic should be " + _topicBoolMqtt.Identifier + ", but is " + msg.Identifier);
        }

        [Test(Description = "Receive a Message via Topic")]
        public void Receive_SubscribedTopic_NotIIdentifierMessage_TopicRaisesReceiveEvent()
        {
            //Arrange
            var wait = new AutoResetEvent(false);
            _topicBoolIByteSerializable.Received += (sender, eventArgs) => { wait.Set(); };
            _topicBoolIByteSerializable.Received += OnReceivedIByteSerializMessage;

            //Act
            _driver.Receive(new MqttApplicationMessage()
            {
                Topic = _driver.Identifier + _topicBoolIByteSerializable.Identifier,
                PayloadSegment = new BoolByteSerializableMessage { Message = MESSAGE_VALUE }.ToBytes()
            });

            //Assert 1
            Assert.That(wait.WaitOne(TimeSpan.FromSeconds(TIMEOUT)), "Received Event was not raised");
        }

        private void OnReceivedIByteSerializMessage(object sender, object e)
        {
            //Assert 2
            var msg = e as BoolByteSerializableMessage;
            Assert.That(msg != null, "Not the right type: Should be TestMessage but is " + e.GetType());
            Assert.That(msg.Message == MESSAGE_VALUE, "Message should " + MESSAGE_VALUE + ", but is " + msg.Message);
        }

        [Test(Description = "No Received-Event will be raised, if the received Topic is no child")]
        public void Receive_NotSubscribedTopic_ReceivedEventWillNotBeRaised()
        {
            //Arrange
            var msg = new BoolMqttMessage { Message = MESSAGE_VALUE, Identifier = "shouldNotBeFound" };
            var wait = new AutoResetEvent(false);
            _driver.Received += (sender, eventArgs) => { wait.Set(); };

            //Act
            _driver.Receive(new MqttApplicationMessage()
            {
                Topic = _driver.Identifier + "shouldNotBeFound",
                PayloadSegment = msg.ToBytes()
            });

            //Assert
            Assert.That(!wait.WaitOne(TimeSpan.FromSeconds(TIMEOUT)), "Received Event was raised, although topic is not the driver's child");
        }

    }
}

