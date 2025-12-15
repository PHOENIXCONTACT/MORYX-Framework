// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.Drivers.Mqtt.Topics;
using NUnit.Framework;

namespace Moryx.Drivers.Mqtt.Tests
{
    [TestFixture]
    public class TestMqttTopic
    {
        private string _topicWithPlaceholders;
        private List<string> _placeHolderNames;

        [SetUp]
        public void Setup()
        {
            _placeHolderNames = new List<string> { "{MachineNR}", "{PC_Name}" };
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
        [TestCase("#/abs/34", "as/ff/abs/34", true)]
        [TestCase("#/abs/34", "as/ff/34", false)]
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

    }
}
