// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.TestTools;
using Moryx.Drivers.Mqtt.Tests.TestMessages;
using Moryx.Drivers.Mqtt.Topics;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.TestTools.UnitTest;
using Moryx.Tools;
using MQTTnet;
using MQTTnet.Formatter;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Moryx.Drivers.Mqtt.Tests
{
    [TestFixture(MqttProtocolVersion.V310)]
    [TestFixture(MqttProtocolVersion.V311)]
    [TestFixture(MqttProtocolVersion.V500)]
    public class TestDriverMqttChannelFunctions
    {
        private const int TIMEOUT = 2;
        private const string TOPIC = "sdf/weras";

        private Mock<IMqttClient> _mockClient;
        private MqttDriver _driver;
        private MqttTopic _topicBoolMqtt;
        private MqttTopic _topicPlaceholder;
        private MessageForPlaceholderMessages _placeholderMessages;
        private MqttProtocolVersion _version;

        public TestDriverMqttChannelFunctions(MqttProtocolVersion version) => _version = version;

        [SetUp]
        public async Task Setup()
        {
            ReflectionTool.TestMode = true;
            _driver = CreateMqttDriver();
            _mockClient = SetupMqttClientMock();

            //Initialize MqttDriver
            _driver.InitializeForTest(_mockClient.Object);
            await ((IAsyncPlugin)_driver).StartAsync();
            _driver.OnConnected(new MqttClientConnectedEventArgs(new MqttClientConnectResult())).Wait();

            //Setup test topic-Resources
            _topicBoolMqtt = new MqttTopicIByteSerializable
            {
                Identifier = "test",
                MessageName = nameof(BoolMqttMessage),
            };
            await SetupTopic(_topicBoolMqtt);

            _topicPlaceholder = new MqttTopicJson
            {
                MessageName = nameof(MessageForPlaceholderMessages),
                Identifier = "abc/{PcName}/fsd/{AdapterNumber}",
            };
            await SetupTopic(_topicPlaceholder);

            _placeholderMessages = new MessageForPlaceholderMessages
            {
                AdapterNumber = 5,
                PcName = "Pc5",
                Value = 42,
                Identity = new ProductIdentity("adf", 3),
                ClassProperty = new TestClass { Test = "testproperty" }
            };
        }

        private MqttDriver CreateMqttDriver()
        {
            return new MqttDriver
            {
                Identifier = "topicDriver",
                Id = 4,
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
                Channels = new ReferenceCollectionMock<MqttTopic>(),
                MqttVersion = _version,
                BrokerUrl = "mock"
            };
        }

        private static Mock<IMqttClient> SetupMqttClientMock()
        {
            var mock = new Mock<IMqttClient>();
            mock.Setup(m => m.ConnectAsync(It.IsAny<MqttClientOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MqttClientConnectResult());
            mock.Setup(m => m.SubscribeAsync(It.IsAny<MqttClientSubscribeOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MqttClientSubscribeResult(0, Array.Empty<MqttClientSubscribeResultItem>(), "", Array.Empty<MqttUserProperty>()));
            mock.Setup(m => m.UnsubscribeAsync(It.IsAny<MqttClientUnsubscribeOptions>(), CancellationToken.None))
                .ReturnsAsync(new MqttClientUnsubscribeResult(0, Array.Empty<MqttClientUnsubscribeResultItem>(), "", Array.Empty<MqttUserProperty>()));
            return mock;
        }

        private async Task SetupTopic(MqttTopic topic)
        {
            topic.Parent = _driver;
            topic.ParallelOperations = new NotSoParallelOps();
            topic.Logger = new ModuleLogger("Dummy", new NullLoggerFactory());
            _driver.Channels.Add(topic);
            await ((IAsyncInitializable)topic).InitializeAsync();
            await((IAsyncPlugin)topic).StartAsync();
        }

        [Test(Description = "Find Channel with one parameter")]
        public void Channel_FindChannel_TChannel()
        {
            var c = _driver.Channel(_topicBoolMqtt.Identifier);
            Assert.That(c != null);
            Assert.That(c.Identifier.Equals(_topicBoolMqtt.Identifier));
        }

        [Test(Description = "Return null, if identifier does not exist")]
        public void Channel_NotFindChannel_IdentifierDoesNotExist()
        {
            var c = _driver.Channel("doesNotExist");
            Assert.That(c == null);
        }

        [Test(Description = "Change topicName")]
        public void ChangeChannelName()
        {
            //Arrange
            var oldTopic = _topicBoolMqtt.Identifier;
            const string newTopic = "newTopicName";
            _mockClient.Setup(m => m.SubscribeAsync(It.IsAny<MqttClientSubscribeOptions>(), CancellationToken.None))
                .ReturnsAsync(new MqttClientSubscribeResult(0, Array.Empty<MqttClientSubscribeResultItem>(), "", Array.Empty<MqttUserProperty>()))
                .Callback<MqttClientSubscribeOptions, CancellationToken>((options, token) => CheckAddedTopic(options, newTopic));
            _mockClient.Setup(m => m.UnsubscribeAsync(It.IsAny<MqttClientUnsubscribeOptions>(), CancellationToken.None))
                .ReturnsAsync(new MqttClientUnsubscribeResult(0, Array.Empty<MqttClientUnsubscribeResultItem>(), "", Array.Empty<MqttUserProperty>()))
                .Callback<MqttClientUnsubscribeOptions, CancellationToken>((options, token) => CheckUnsubscribedTopic(options, oldTopic));

            //Act
            _topicBoolMqtt.Identifier = newTopic;

            //Assert I
            _mockClient.Verify(m => m.SubscribeAsync(It.IsAny<MqttClientSubscribeOptions>(),
                CancellationToken.None));
            _mockClient.Verify(m => m.UnsubscribeAsync(It.IsAny<MqttClientUnsubscribeOptions>(),
                CancellationToken.None));
        }

        [Test(Description = "After a MqttTopic is added to a MqttDriver, the driver subscribes to the topic")]
        public async Task AfterAddingATopicDriverSubscribesToIt()
        {
            //Arrange
            var topic = new MqttTopicIByteSerializable
            {
                Identifier = "test1",
                MessageName = nameof(BoolMqttMessage)
            };
            await SetupTopic(topic);

            _mockClient.Setup(m => m.SubscribeAsync(It.IsAny<MqttClientSubscribeOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MqttClientSubscribeResult(0, Array.Empty<MqttClientSubscribeResultItem>(), "", Array.Empty<MqttUserProperty>()))
                .Callback<MqttClientSubscribeOptions, CancellationToken>((options, token) =>
                    CheckAddedTopic(options, topic.Identifier));
            //Act
            await ((IAsyncPlugin)topic).StartAsync();

            //Assert I
            _mockClient.Verify(m => m.SubscribeAsync(It.IsAny<MqttClientSubscribeOptions>(),
                CancellationToken.None));
        }

        private void CheckAddedTopic(MqttClientSubscribeOptions options, string topic)
        {
            //Assert II
            var topicFilter = options.TopicFilters.First();
            Assert.That(_driver.Identifier + topic, Is.EqualTo(topicFilter.Topic));
            Assert.That(MqttQualityOfServiceLevel.ExactlyOnce, Is.EqualTo(topicFilter.QualityOfServiceLevel));
        }

        private void CheckUnsubscribedTopic(MqttClientUnsubscribeOptions options, string topic)
        {
            //Assert III
            Assert.That(_driver.Identifier + topic, Is.EqualTo(options.TopicFilters.First()),
                "wrong topic was unsubscribed");
        }

        [Test]
        [TestCase("foo#/hi", nameof(BoolByteSerializableMessage), false)]
        [TestCase("foo{sd}", nameof(BoolByteSerializableMessage), false)]
        [TestCase("+foo", nameof(BoolByteSerializableMessage), false)]
        [TestCase("foo/+sdf/{sd}/as", nameof(BoolByteSerializableMessage), false)]
        [TestCase("foo/#", nameof(BoolMqttMessage), true)]
        [TestCase("foo/+/sd/as", nameof(BoolMqttMessage), true)]
        [TestCase("{PcName}/{Value}/asd", nameof(MessageForPlaceholderMessages), true,
            "+/+/asd")]
        [TestCase("{PcName}/{Value.Value}/asd", nameof(MessageForPlaceholderMessages), true,
            "+/+/asd")]
        public async Task TopicSetIdentifier_IdentifierCanOnlyBeSetToMqttConformTopics(
            string newTopic, string messageName, bool shouldBeSet, string subscribedTopic = "")
        {
            //Arrange
            if (subscribedTopic.Equals(""))
            {
                subscribedTopic = newTopic;
            }

            var topic = new MqttTopicJson
            {
                MessageName = messageName,
                Identifier = "text",
            };
            await SetupTopic(topic);
            _mockClient.Setup(m => m.SubscribeAsync(It.IsAny<MqttClientSubscribeOptions>(), CancellationToken.None))
                .ReturnsAsync(new MqttClientSubscribeResult(0, Array.Empty<MqttClientSubscribeResultItem>(), "", Array.Empty<MqttUserProperty>()))
                .Callback<MqttClientSubscribeOptions, CancellationToken>((options, token) =>
                    CheckAddedTopic(options, subscribedTopic));

            //Act
            topic.Identifier = newTopic;
            var topicWasSet = topic.Identifier.Equals(newTopic);

            //Assert
            Assert.That(shouldBeSet, Is.EqualTo(topicWasSet));
            _mockClient.Verify(m => m.SubscribeAsync(It.IsAny<MqttClientSubscribeOptions>(),
                CancellationToken.None));
        }

        [Test]
        public void ReceiveMessageContainingPlaceholders()
        {
            //Arrange
            _topicPlaceholder.Identifier = "{ClassProperty.Test}/asd/{PcName}/{AdapterNumber}";
            var msg = new MessageForPlaceholderMessages
            {
                Value = _placeholderMessages.Value,
                ClassProperty = new TestClass()
            };
            var topic = _driver.Identifier + _topicPlaceholder.Identifier
                .Replace("{PcName}", _placeholderMessages.PcName)
                .Replace("{AdapterNumber}", _placeholderMessages.AdapterNumber.ToString(CultureInfo.InvariantCulture))
                .Replace("{ClassProperty.Test}", _placeholderMessages.ClassProperty.Test);
            var wait = new AutoResetEvent(false);
            _topicPlaceholder.Received += (sender, eventArgs) => { wait.Set(); };
            _topicPlaceholder.Received += OnPlaceholderMessageReceived;

            //Act
            _driver.Receive(new MqttApplicationMessage()
            {
                Topic = topic,
                PayloadSegment = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(msg))
            });

            //Assert 1
            Assert.That(wait.WaitOne(TimeSpan.FromSeconds(TIMEOUT)), "Received Event was not raised");
        }

        private void OnPlaceholderMessageReceived(object sender, object e)
        {
            var msg = e as MessageForPlaceholderMessages;
            Assert.That(msg, Is.Not.Null);
            Assert.That(_placeholderMessages.PcName, Is.EqualTo(msg.PcName), "placeholder for string was not matched");
            Assert.That(_placeholderMessages.AdapterNumber, Is.EqualTo(msg.AdapterNumber), "placeholder for int was not matched");
            Assert.That(_placeholderMessages.Value, Is.EqualTo(msg.Value), "value was not sent");
            Assert.That(_placeholderMessages.ClassProperty.Test, Is.EqualTo(msg.ClassProperty.Test),
                "relative placeholder was not matched");
        }

        [Test]
        [TestCase("abc/{PcName}/fsd/{AdapterNumber}")]
        [TestCase("abc/{PcName}/fsd/{Value}")]
        [TestCase("abc/{Identity.Revision}")]
        [TestCase("{Identity.Identifier}/asd/Value")]
        public void SendMessageContainingPlaceholders(string topic)
        {
            //Arrange
            _topicPlaceholder.Identifier = topic;
            _mockClient.Setup(m => m.PublishAsync(It.IsAny<MqttApplicationMessage>(), CancellationToken.None))
                .ReturnsAsync(new MqttClientPublishResult(0, MqttClientPublishReasonCode.Success, "", Array.Empty<MqttUserProperty>()))
                .Callback<MqttApplicationMessage, CancellationToken>((applicationMessage, token) =>
                    CheckSentMessageWithPlaceholder(applicationMessage, _placeholderMessages, topic));

            //Act
            _topicPlaceholder.Send(_placeholderMessages);

            //Assert I
            _mockClient.Verify(m => m.PublishAsync(It.IsAny<MqttApplicationMessage>(),
                CancellationToken.None));
        }
        private void CheckSentMessageWithPlaceholder(MqttApplicationMessage applicationMessage,
            MessageForPlaceholderMessages msg, string topicIdentifier)
        {
            var topic = _driver.Identifier + topicIdentifier.Replace("{PcName}", msg.PcName)
                .Replace("{AdapterNumber}", msg.AdapterNumber.ToString(CultureInfo.InvariantCulture))
                .Replace("{Value}", msg.Value.ToString(CultureInfo.InvariantCulture))
                .Replace("{Identity.Revision}", msg.Identity.Revision.ToString(CultureInfo.InvariantCulture))
                .Replace("{Identity.Identifier}", msg.Identity.Identifier);
            Assert.That(topic, Is.EqualTo(applicationMessage.Topic), "topic was wrongly built, placeholders weren't replaced with the right values");
        }

        [Test]
        public void SendMessageContainingPlaceholders_MessageCannotBeSend_MissingProperties()
        {
            //Arrange
            _topicPlaceholder.Identifier += "/{placeholderNoFound}";

            //Act + Assert
            Assert.Throws<ArgumentException>(() => _topicPlaceholder.Send(_placeholderMessages));
        }

        [Test]
        [TestCase("sdf/+", "sdf/qew")]
        [TestCase("sdf/+/sd", "sdf/qw/sd")]
        [TestCase("sdf/#/asd", "sdf/fof/sdf/asd")]
        [TestCase("sdf/#/asd", "sdf/fof/asd")]
        public async Task SendMessageContainingWildcards(string topicResource, string topicMsg)
        {
            var msg = new BoolMqttMessage { Identifier = topicMsg };
            var mqttTopic = new MqttTopicIByteSerializable
            {
                Identifier = topicResource,
                MessageName = msg.GetType().Name,
            };
            await SetupTopic(mqttTopic);
            _mockClient.Setup(m => m.PublishAsync(It.IsAny<MqttApplicationMessage>(),
                CancellationToken.None)).Callback<MqttApplicationMessage, CancellationToken>(
                (applicationMessage, token) =>
                    CheckMessageSentWithWildcards(applicationMessage, topicMsg));

            //Act
            await mqttTopic.SendAsync(msg);

            //Assert
            _mockClient.Verify(m => m.PublishAsync(It.IsAny<MqttApplicationMessage>(),
                CancellationToken.None));
        }

        private void CheckMessageSentWithWildcards(MqttApplicationMessage applicationMessage, string topicMsg)
        {
            Assert.That(_driver.Identifier + topicMsg, Is.EqualTo(applicationMessage.Topic), "wrong topic was used");
        }

        [Test]
        [TestCase("sdf/+", "sdf/{s}")]
        [TestCase("sdf/+", "sdf/#")]
        [TestCase("sdf/+", "sdf/+")]
        [TestCase("sdf/+", "sdf/sdwe/sdfa")]
        [TestCase("sdf/+", "sdf")]
        public async Task SendMessageContainingWildcards_MessageCannotBeSent_WrongIdentifierInMessage(
            string topicResource, string topicMsg)
        {
            //Arrange
            var msg = new BoolMqttMessage { Identifier = topicMsg };
            var mqttTopic = new MqttTopicIByteSerializable
            {
                Identifier = topicResource,
                MessageName = msg.GetType().Name,
            };
            await SetupTopic(mqttTopic);

            //Act + Assert
            Assert.Throws<ArgumentException>(() => mqttTopic.Send(msg));
        }

        [Test]
        public async Task SendMessageContainingWildcards_MessageCannotBeSent_WrongMessageType()
        {
            //Arrange
            var msg = new BoolByteSerializableMessage();
            var mqttTopic = new MqttTopicIByteSerializable
            {
                Identifier = "sdf/+",
                MessageName = msg.GetType().Name,
            };
            await SetupTopic(mqttTopic);

            //Act + Assert
            Assert.Throws<ArgumentException>(() => mqttTopic.Send(msg));
        }

        [Test]
        [TestCase("ShouldNotBeFound", false, typeof(MqttTopicJson))]
        [TestCase(nameof(JsonMessageTest), true, typeof(MqttTopicJson))]
        [TestCase(nameof(BoolByteSerializableMessage), true, typeof(MqttTopicIByteSerializable))]
        public void MqttTopicMessageNameCanOnlyBeChangedIfTypeExists(string messageName, bool shouldBeChanged, Type topicType)
        {
            //Arrange
            var topic = (MqttTopic)Activator.CreateInstance(topicType);
            topic.Logger = new ModuleLogger("Dummy", new NullLoggerFactory());

            //Act
            topic.MessageName = messageName;

            //Assert
            var changed = topic.MessageName != null;
            Assert.That(shouldBeChanged, Is.EqualTo(changed));
        }

        [Test]
        [TestCase(nameof(JsonMessageTest), false, "sdf/{foo}")]
        [TestCase(nameof(JsonMessageTest), true, "{Age}/sdf")]
        [TestCase(nameof(MessageForPlaceholderMessages), true, "s/{PcName}/df/{AdapterNumber}/{Value}")]
        public void MqttTopicMessageNameCanOnlyBeChangedIfItMatchesTopic(string messageName, bool shouldBeChanged, string topic)
        {
            //Arrange
            var topicResource = new MqttTopicJson { Identifier = topic, Logger = new ModuleLogger("Dummy", new NullLoggerFactory()) };

            //Act
            topicResource.MessageName = messageName;

            //Assert
            var changed = topicResource.MessageName != null;
            Assert.That(shouldBeChanged, Is.EqualTo(changed));
        }

        [Test]
        public async Task ReceiveIdentifierMessage_SetActualReceivedTopicAsIdentifier()
        {
            var mqttTopic = new MqttTopicIByteSerializable
            {
                Identifier = "sdf/+",
                MessageName = nameof(BoolMqttMessage)
            };
            await SetupTopic(mqttTopic);
            var msg = new BoolMqttMessage { Message = true };
            mqttTopic.Received += CheckReceivedIIdentifierMessage;
            var wait = new AutoResetEvent(false);
            mqttTopic.Received += (sender, eventArgs) => { wait.Set(); };

            //Act
            _driver.Receive(new MqttApplicationMessage()
            {
                Topic = _driver.Identifier + TOPIC,
                PayloadSegment = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(msg))
            });

            //Assert 1
            Assert.That(wait.WaitOne(TimeSpan.FromSeconds(TIMEOUT)), "Received Event was not raised");
        }

        private void CheckReceivedIIdentifierMessage(object sender, object e)
        {
            //Assert 2
            var msg = e as BoolMqttMessage;
            Assert.That(msg, Is.Not.Null, "Message has wrong datatype");
            Assert.That(TOPIC, Is.EqualTo(msg.Identifier), "Message has wrong Identifier");
        }
    }
}

