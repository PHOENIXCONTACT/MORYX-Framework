// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.TestTools;
using Moryx.Drivers.Mqtt;
using Moryx.Drivers.Mqtt.MqttTopics;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Tools;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Packets;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace Moryx.Resources.Mqtt.Tests
{
    [TestFixture(MqttProtocolVersion.V310)]
    [TestFixture(MqttProtocolVersion.V311)]
    [TestFixture(MqttProtocolVersion.V500)]
    public class TestDriverMqttJsonTopic
    {
        private const int TIMEOUT = 2;
        private const string MESSAGE_VALUE_NAME = "Elsa";
        private const int MESSAGE_VALUE_AGE = 19;
        private Mock<IMqttClient> _mockClient;
        private MqttDriver _driver;
        private MqttTopic _mqttTopicCamel;
        private MqttTopic _mqttTopicPascal;
        private MqttProtocolVersion _version;

        public TestDriverMqttJsonTopic(MqttProtocolVersion version) => _version = version;

        [SetUp]
        public void Setup()
        {
            ReflectionTool.TestMode = true;

            _mqttTopicCamel = new MqttTopicJson()
            {
                Identifier = "JsonMqttCamel",
                MessageName = nameof(JsonMessageTest),
                Format = JsonFormat.camelCase
            };
            _mqttTopicPascal = new MqttTopicJson()
            {
                Identifier = "JsonMqttPascal",
                MessageName = nameof(JsonMessageTest)
            };

            ((IInitializable)_mqttTopicCamel).Initialize();
            ((IInitializable)_mqttTopicPascal).Initialize();

            _driver = new MqttDriver
            {
                Identifier = "topicDriver/",
                Id = 4,
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
                Channels = new ReferenceCollectionMock<MqttTopic> { _mqttTopicCamel, _mqttTopicPascal },
                MqttVersion = _version
            };

            _mockClient = new Mock<IMqttClient>();
            _mockClient.Setup(m => m.ConnectAsync(It.Is(CorrectClientOptions()), CancellationToken.None))
                .ReturnsAsync(new MqttClientConnectResult());
            _mockClient.Setup(m => m.SubscribeAsync(It.IsAny<MqttClientSubscribeOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MqttClientSubscribeResult(0, Array.Empty<MqttClientSubscribeResultItem>(), "", Array.Empty<MqttUserProperty>()));

            _driver.InitializeForTest(_mockClient.Object);
            ((IPlugin)_driver).Start();
            _driver.OnConnected(new MqttClientConnectedEventArgs(new MqttClientConnectResult())).Wait();
            _mqttTopicCamel.Parent = _driver;
            _mqttTopicPascal.Parent = _driver;
        }

        private Expression<Func<MqttClientOptions, bool>> CorrectClientOptions()
        {
            return o => o.ProtocolVersion == _driver.MqttVersion && o.CleanSession == !_driver.ReconnectWithoutCleanSession
                        && o.ClientId == $"{System.Net.Dns.GetHostName()}-{_driver.Id}-{_driver.Name}" && (o.ChannelOptions as MqttClientTcpOptions).Server == _driver.BrokerUrl &&
                        (o.ChannelOptions as MqttClientTcpOptions).Port == _driver.Port;
        }

        [Test(Description = "Publish Json Message using the MqttTopicJson")]
        public void Send_UsingMqttTopicJson_Topic_QOS_Message()
        {
            //Arrange
            _ = _mockClient.Setup(m => m.PublishAsync(
                    It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>()))
                .Callback<MqttApplicationMessage, CancellationToken>(CheckSentMessage);

            //Act
            _mqttTopicPascal.Send(new JsonMessageTest { Name = MESSAGE_VALUE_NAME, Age = MESSAGE_VALUE_AGE });
            _mqttTopicCamel.Send(new JsonMessageTest { Name = MESSAGE_VALUE_NAME, Age = MESSAGE_VALUE_AGE });

            //Assert 1
            _mockClient.Verify((m => m.PublishAsync(
                It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>())));

        }

        private void CheckSentMessage(MqttApplicationMessage sentMsg, CancellationToken token)
        {
            var msg = new JsonMessageTest();
            var payload = Encoding.UTF8.GetString(sentMsg.Payload, 0, sentMsg.Payload.Length);
            if (sentMsg.Topic == _driver.Identifier + _mqttTopicCamel.Identifier)
            {
                Assert.That(payload.Contains(nameof(JsonMessageTest.Age).ToLower()));
                Assert.That(payload.Contains(nameof(JsonMessageTest.Age).ToLower()));
                JsonConvert.PopulateObject(payload, msg, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }
            else if (sentMsg.Topic == _driver.Identifier + _mqttTopicPascal.Identifier)
            {
                Assert.That(payload.Contains(nameof(JsonMessageTest.Age)));
                Assert.That(payload.Contains(nameof(JsonMessageTest.Age)));
                JsonConvert.PopulateObject(payload, msg);
            }
            else
            {
                Assert.Fail("Message was not on any of the expected topics");
            }

            Assert.That(msg.Name.Equals(MESSAGE_VALUE_NAME), "Property should be " + MESSAGE_VALUE_NAME + ", but is " + msg.Name);
            Assert.That(msg.Age == MESSAGE_VALUE_AGE, "Property should be " + MESSAGE_VALUE_AGE + ", but is " + msg.Age);
        }

        [Test(Description = "Receive a Json Message via MqttTopicJson")]
        public void Receive_SubscribedTopic_MqttTopicJson_TopicRaisesReceiveEvent()
        {
            //Arrange
            var waitPascal = new AutoResetEvent(false);
            var waitCamel = new AutoResetEvent(false);
            _mqttTopicPascal.Received += (sender, eventArgs) => { waitPascal.Set(); };
            _mqttTopicPascal.Received += OnReceivedMessage;
            _mqttTopicCamel.Received += (sender, eventArgs) => { waitCamel.Set(); };
            _mqttTopicCamel.Received += OnReceivedMessage;

            var message = new JsonMessageTest
            {
                Age = MESSAGE_VALUE_AGE,
                Name = MESSAGE_VALUE_NAME
            };
            var pascalJson = JsonConvert.SerializeObject(message);
            var camelJson = JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            //Act
            _driver.Receive(_driver.Identifier + _mqttTopicPascal.Identifier, Encoding.ASCII.GetBytes(pascalJson));
            _driver.Receive(_driver.Identifier + _mqttTopicCamel.Identifier, Encoding.ASCII.GetBytes(camelJson));

            //Assert 1
            Assert.That(waitCamel.WaitOne(TimeSpan.FromSeconds(TIMEOUT)), "Received Event was not raised");
        }

        private void OnReceivedMessage(object sender, object e)
        {
            //Assert 2
            var msg = e as JsonMessageTest;
            Assert.That(msg != null, "Not the right type: Should be TestMessage but is " + e.GetType());
            Assert.That(msg.Name.Equals(MESSAGE_VALUE_NAME), "Property should " + MESSAGE_VALUE_NAME + ", but is " + msg.Name);
            Assert.That(msg.Age == MESSAGE_VALUE_AGE, "Property should " + MESSAGE_VALUE_AGE + ", but is " + msg.Age);
        }
    }

    public class JsonMessageTest
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}

