// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Buffers;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.TestTools;
using Moryx.Drivers.Mqtt.Topics;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Tools;
using MQTTnet;
using MQTTnet.Formatter;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using NUnit.Framework;

namespace Moryx.Drivers.Mqtt.Tests;

[TestFixture(MqttProtocolVersion.V310)]
[TestFixture(MqttProtocolVersion.V311)]
[TestFixture(MqttProtocolVersion.V500)]
public class TestDriverMqttPrimitiveDatatypeTopic
{
    private const int TIMEOUT = 2;
    private const string MESSAGE_VALUE_STRING = "Elsa";
    private const int MESSAGE_VALUE_INT = 19;
    private Mock<IMqttClient> _mockClient;
    private MqttDriver _driver;
    private MqttTopic _mqttTopicInt;
    private MqttTopic _mqttTopicString;
    private readonly MqttProtocolVersion _version;

    public TestDriverMqttPrimitiveDatatypeTopic(MqttProtocolVersion version) => _version = version;

    [SetUp]
    public async Task Setup()
    {
        ReflectionTool.TestMode = true;

        _mqttTopicInt = new MqttTopicPrimitive()
        {
            Identifier = "MessageInt",
            MessageName = "System." + nameof(Int32),
            MessageType = typeof(int)
        };

        _mqttTopicString = new MqttTopicPrimitive()
        {
            Identifier = "MessageString",
            MessageName = "System." + nameof(String),
            MessageType = typeof(string)
        };

        await ((IAsyncInitializable)_mqttTopicInt).InitializeAsync();
        await ((IAsyncInitializable)_mqttTopicString).InitializeAsync();

        _driver = new MqttDriver
        {
            Identifier = "topicDriver/",
            Id = 4,
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
            Channels = new ReferenceCollectionMock<MqttTopic> { _mqttTopicInt, _mqttTopicString },
            MqttVersion = _version,
            BrokerUrl = "mock"
        };

        _mockClient = new Mock<IMqttClient>();
        _mockClient.Setup(m => m.ConnectAsync(It.Is(CorrectClientOptions()), CancellationToken.None))
            .ReturnsAsync(new MqttClientConnectResult());
        _mockClient.Setup(m => m.SubscribeAsync(It.IsAny<MqttClientSubscribeOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MqttClientSubscribeResult(0, Array.Empty<MqttClientSubscribeResultItem>(), "", Array.Empty<MqttUserProperty>()));

        _driver.InitializeForTest(_mockClient.Object);
        await ((IAsyncPlugin)_driver).StartAsync();
        _driver.OnConnected(new MqttClientConnectedEventArgs(new MqttClientConnectResult())).Wait();
        _mqttTopicInt.Parent = _driver;
        _mqttTopicString.Parent = _driver;
    }

    private Expression<Func<MqttClientOptions, bool>> CorrectClientOptions()
    {
        return o => o.ProtocolVersion == _driver.MqttVersion && o.CleanSession == !_driver.ReconnectWithoutCleanSession
                                                             && o.ClientId == $"{System.Net.Dns.GetHostName()}-{_driver.Id}-{_driver.Name}";
    }

    [Test(Description = "Publish int32 Message using the MqttTopicPrimitive")]
    public void SendInt_UsingMqttTopicPrimitive_Topic_QOS_Message()
    {
        //Arrange
        _mockClient.Setup(m => m.PublishAsync(
                It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>()))
            .Callback<MqttApplicationMessage, CancellationToken>((sentMsg, token)
                => SendMessagIntIConvertible(sentMsg, token));

        //Act
        _mqttTopicInt.Send(MESSAGE_VALUE_INT);

        //Assert 1
        _mockClient.Verify((m => m.PublishAsync(
            It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>())));

    }

    private void SendMessagIntIConvertible(MqttApplicationMessage sentMsg, CancellationToken token)
    {
        //Assert 2
        Assert.That(token, Is.EqualTo(CancellationToken.None));
        Assert.That(sentMsg.Topic.Equals(_driver.Identifier + _mqttTopicInt.Identifier),
            "Topic should be " + _driver.Identifier + _mqttTopicInt.Identifier + ", but is " + sentMsg.Topic);
        Assert.That(sentMsg.QualityOfServiceLevel, Is.EqualTo(MqttQualityOfServiceLevel.ExactlyOnce),
            "Qos should be ExactlyOnce, but is " + sentMsg.QualityOfServiceLevel);
        var msg = BitConverter.ToInt32(sentMsg.Payload.ToArray(), 0);
        Assert.That(msg == MESSAGE_VALUE_INT, "Message should be " + MESSAGE_VALUE_INT + ", but is " + msg);
    }

    [Test(Description = "Publish string Message using the MqttTopicPrimitive")]
    public void SendString_UsingMqttTopicPrimitive_Topic_QOS_Message()
    {
        //Arrange
        _mockClient.Setup(m => m.PublishAsync(
                It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>()))
            .Callback<MqttApplicationMessage, CancellationToken>((sentMsg, token)
                => SendMessagStringIConvertible(sentMsg, token));

        //Act
        _mqttTopicString.Send(MESSAGE_VALUE_STRING);

        //Assert 1
        _mockClient.Verify((m => m.PublishAsync(
            It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>())));

    }

    private void SendMessagStringIConvertible(MqttApplicationMessage sentMsg, CancellationToken token)
    {
        //Assert 2
        Assert.That(token, Is.EqualTo(CancellationToken.None));
        Assert.That(sentMsg.Topic.Equals(_driver.Identifier + _mqttTopicString.Identifier),
            "Topic should be " + _driver.Identifier + _mqttTopicString.Identifier + ", but is " + sentMsg.Topic);
        Assert.That(sentMsg.QualityOfServiceLevel, Is.EqualTo(MqttQualityOfServiceLevel.ExactlyOnce),
            "Qos should be ExactlyOnce, but is " + sentMsg.QualityOfServiceLevel);
        var msg = Encoding.ASCII.GetString(sentMsg.Payload);
        Assert.That(msg.Equals(MESSAGE_VALUE_STRING), "Message should be " + MESSAGE_VALUE_STRING + ", but is " + msg);
    }

    [Test(Description = "Receive a Int-Message via MqttTopicPrimitive")]
    public void ReceiveInt_SubscribedTopic_MqttTopicPrimitive_TopicRaisesReceiveEvent()
    {
        //Arrange
        var wait = new AutoResetEvent(false);
        _mqttTopicInt.Received += (sender, eventArgs) => { wait.Set(); };
        _mqttTopicInt.Received += OnReceivedIntMessage;

        //Act
        _driver.Receive(new MqttApplicationMessage()
        {
            Topic = _driver.Identifier + _mqttTopicInt.Identifier,
            PayloadSegment = BitConverter.GetBytes(MESSAGE_VALUE_INT)
        });

        //Assert 1
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(TIMEOUT)), "Received Event was not raised");
    }

    private void OnReceivedIntMessage(object sender, object e)
    {
        //Assert 2
        var msg = (int)e;
        Assert.That(msg == MESSAGE_VALUE_INT, "Message should be " + MESSAGE_VALUE_INT + ", but is " + msg);
    }

    [Test(Description = "Receive a String-Message via MqttTopicPrimitive")]
    public void ReceiveString_SubscribedTopic_MqttTopicPrimitive_TopicRaisesReceiveEvent()
    {
        //Arrange
        var wait = new AutoResetEvent(false);
        _mqttTopicString.Received += (sender, eventArgs) => { wait.Set(); };
        _mqttTopicString.Received += OnReceivedStringMessage;

        //Act
        _driver.Receive(new MqttApplicationMessage()
        {
            Topic = _driver.Identifier + _mqttTopicString.Identifier,
            PayloadSegment = Encoding.ASCII.GetBytes(MESSAGE_VALUE_STRING)
        });

        //Assert 1
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(TIMEOUT)), "Received Event was not raised");
    }

    private void OnReceivedStringMessage(object sender, object e)
    {
        //Assert 2
        var msg = (string)e;
        Assert.That(msg.Equals(MESSAGE_VALUE_STRING), "Message should be " + MESSAGE_VALUE_STRING + ", but is " + msg);
    }

    [Test]
    [TestCase(typeof(int))]
    [TestCase(typeof(string))]
    [TestCase(typeof(bool))]
    public void ChangeMessageName(Type type)
    {
        //Arrange
        var topic = new MqttTopicPrimitive();

        //Act
        topic.MessageNameEnum = Type.GetTypeCode(type);

        //Assert
        Assert.That(type, Is.EqualTo(topic.MessageType));
        Assert.That(type.Name, Is.EqualTo(topic.MessageName));
        Assert.That(Type.GetTypeCode(type), Is.EqualTo(topic.MessageNameEnum));
    }
}