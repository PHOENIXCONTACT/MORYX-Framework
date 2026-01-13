// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.AbstractionLayer.TestTools;
using Moryx.Drivers.Mqtt.Tests.TestMessages;
using Moryx.Drivers.Mqtt.Topics;
using Moryx.Logging;
using NUnit.Framework;

namespace Moryx.Drivers.Mqtt.Tests;

[TestFixture]
public class TestMqttTopic
{
    private string _topicWithPlaceholders;
    private List<string> _placeHolderNames;

    [SetUp]
    public void Setup()
    {
        _placeHolderNames = ["{MachineNR}", "{PC_Name}"];
        const string topicBeginning = "abc";
        _topicWithPlaceholders = topicBeginning;
        foreach (var n in _placeHolderNames)
        {
            _topicWithPlaceholders += "/" + n;
        }
    }

    [Test(Description = "Tests if a received topic can be matched to the corresponding TopicResource")]
    [TestCase("{foo}/{foo1}/abs/34", "as/ff/abs/34", true)]
    [TestCase("{foo}/{foo1}/abs/34", "as/abs/34", false)]
    [TestCase("{foo}/{foo1}/abs/34", "as/ff/abs/sdf/34", false)]
    [TestCase("+/+/abs/34", "as/ff/abs/34", true)]
    [TestCase("+/abs/34", "as/ff/34", false)]
    [TestCase("abs/34/+", "abs/34/sdf/wwe", false)]
    [TestCase("{foo}/{foo1.hjh}/abs/+", "as/ff/abs/34", true)]
    public void MessageBelongsToThisTopic(string topicName, string receivedTopic, bool result)
    {
        //Arrange
        var topicResource = new MqttTopicIByteSerializable { Identifier = topicName };

        //Act
        var messageBelongsToTopic = topicResource.Matches(receivedTopic);

        //Assert
        Assert.That(result, Is.EqualTo(messageBelongsToTopic), "Matching is wrong");
    }

    [Test]
    public void GetPlaceHolderNames()
    {
        //Arrange
        var topicResource = new MqttTopicIByteSerializable
        {
            Identifier = _topicWithPlaceholders
        };

        //Act
        var allNames = topicResource.RegexTopic.GetGroupNames();
        var names = new string[allNames.Length - 1];
        Array.Copy(allNames, 1, names, 0, names.Length);

        //Assert
        Assert.That(_placeHolderNames.Count, Is.EqualTo(names.Length));
        foreach (var n in names)
        {
            Assert.That(_placeHolderNames.Contains("{" + n + "}"));
        }
    }

    [TestCase("#", "#")]
    [TestCase("+", "+")]
    [TestCase("a/#", "a/#")]
    [TestCase("a/+", "a/+")]
    [TestCase("+/a", "+/a")]
    [TestCase("{abc}", "+")]
    [TestCase("{abc}/{cde}", "+/+")]
    [TestCase("{abc}/{Variable|#}", "+/#")]
    [TestCase("abc/{Var.Field}", "abc/+")]
    [TestCase("abc/{Var.Field|#}", "abc/#")]
    [TestCase("1/{Var.Field|#}", "1/#")]
    [TestCase("colon:and/dashes-allowed", "colon:and/dashes-allowed")]
    public void SubscribedTopicIsAssignedCorrectlyForValidTopics(string identifier, string expected)
    {

        //Arrange
        var driver = new MqttDriver
        {
            Identifier = "topicDriver/",
            Id = 4,
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
            Channels = new ReferenceCollectionMock<MqttTopic> { },
            BrokerUrl = "mock"
        };
        var topic = new MqttTopicJson()
        {
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
            Parent = driver,
        };
        driver.Channels.Add(topic);

        //Act

        topic.Identifier = identifier;

        //Assert
        Assert.That(topic.SubscribedTopic, Is.EqualTo(expected));
    }

    [TestCase("asdf+/asdf", Description = "+ must be complete topic level")]
    [TestCase("asdf#", Description = "# must be a complete topic level")]
    [TestCase("#/asdf", Description = "# is only allowed at the end")]
    [TestCase("{test|#}/a", Description = "{test|#} is equivalent to an # and is only allowed at the end")]
    [TestCase("asdf{test}", Description = "{test} is equivalent to + and must be an entire level on it's own")]
    [TestCase("asdf asdo/sfon", Description = "We don't allow spaces")]
    public void SubscribedTopicIsNotAssignedForInvalidTopics(string example)
    {
        //Arrange

        var driver = new MqttDriver
        {
            Identifier = "topicDriver/",
            Id = 4,
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
            Channels = new ReferenceCollectionMock<MqttTopic> { },
            BrokerUrl = "mock"
        };
        var topic = new MqttTopicJson()
        {
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
            Parent = driver,
        };

        driver.Channels.Add(topic);
        //Act
        topic.Identifier = example;
        //Assert
        Assert.That(topic.SubscribedTopic, Is.Null);
    }

    public static (string identifier, string topic, MessageForPlaceholderMessages message)[] ParsingExamples => [
        ("a/{PcName}", "a/hello", new MessageForPlaceholderMessages { PcName = "hello"}),
        //("{Identity.Identifier}/{Identity.Revision}", "hello/5", new MessageForPlaceholderMessages() {Identity = new AbstractionLayer.Products.ProductIdentity("hello", 5)}), // asigning nested properties doesn't work yet. Problem in the binding part
        ("34/{PcName|#}", "34/europe/abc-283", new MessageForPlaceholderMessages() {PcName = "europe/abc-283"}),
        ("34/{PcName}", "34/europe", new MessageForPlaceholderMessages() {PcName = "europe"}),
        //("34/{ClassProperty.Test}", "34/europe", new MessageForPlaceholderMessages() {ClassProperty = new() { Test = "europe"} }) // asigning nested properties doesn't work yet. Problem in the binding part
    ];

    [CancelAfter(100)]
    [TestCaseSource(nameof(ParsingExamples))]
    public async Task TestParsing((string identifier, string topic, MessageForPlaceholderMessages message) example, CancellationToken token)
    {
        //Arrange
        var driver = new MqttDriver
        {
            Identifier = "topicDriver/",
            Id = 4,
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
            Channels = new ReferenceCollectionMock<MqttTopic> { },
            BrokerUrl = "mock"
        };
        var jsonTopic = new MqttTopicJson()
        {
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
            MessageName = nameof(MessageForPlaceholderMessages),
            Parent = driver
        };
        driver.Channels.Add(jsonTopic);

        var tcs = new TaskCompletionSource<MessageForPlaceholderMessages>();
        void OnReceived(object sender, object message)
        {
            tcs.SetResult((MessageForPlaceholderMessages)message);
        }
        jsonTopic.Received += OnReceived;
        var payload = new System.Buffers.ReadOnlySequence<byte>(Encoding.UTF8.GetBytes("""
            {}
            """));

        //Act
        jsonTopic.Identifier = example.identifier;
        jsonTopic.OnReceived(example.topic, payload, null, false);

        //Assert

        Assert.That(jsonTopic.Matches(example.topic));
        var parsed = await tcs.Task;

        using var _ = Assert.EnterMultipleScope();
        Assert.That(parsed.PcName, Is.EqualTo(example.message.PcName));
        Assert.That(parsed.Value, Is.EqualTo(example.message.Value));
        Assert.That(parsed.Identity, Is.EqualTo(example.message.Identity));
        Assert.That(parsed.AdapterNumber, Is.EqualTo(example.message.AdapterNumber));
        Assert.That(parsed.ClassProperty?.Test, Is.EqualTo(example.message.ClassProperty?.Test));

    }

}

