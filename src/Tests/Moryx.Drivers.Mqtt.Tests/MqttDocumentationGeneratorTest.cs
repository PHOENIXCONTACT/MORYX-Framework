// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.AbstractionLayer.TestTools;
using Moryx.Drivers.Mqtt.Topics;
using Moryx.Logging;
using NUnit.Framework;

namespace Moryx.Drivers.Mqtt.Tests;

public class MqttDocumentationGeneratorTest
{
    [System.ComponentModel.Description("Example 1 class description")]
    public class Example1
    {
        [JsonIgnore] // Captured from topic
        public string Captured { get; set; }

        [System.ComponentModel.Description("Example field")]
        public string Field { get; set; }
    }

    [Test]
    [Description("Description annotations on json objects should be included in documentation")]
    public void GeneratedOutputContainsDocComments()
    {
        // Arrange
        var sb = new StringBuilder();
        using var writer = new StringWriter(sb);

        var driver = new MqttDriver()
        {
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
            Identifier = "BM/example/",
            Channels = new ReferenceCollectionMock<MqttTopic>(),
        };

        driver.Channels.Add(new MqttTopicJson() { Identifier = "test/{Captured}", MessageName = nameof(Example1) });
        driver.Channels.Add(new MqttTopicJson() { Identifier = "test/{Captured}/Subtopic", MessageName = nameof(Example1) });
        driver.Channels.Add(new MqttTopicJson() { Identifier = "test2/+/example", MessageName = nameof(Example1) });
        var generator = new MqttDocumentationGenerator()
        {
            MqttDriver = driver,
        };

        // Act
        generator.GenerateDocumentation(writer);

        // Assert
        var result = sb.ToString();
        using var _ = Assert.EnterMultipleScope();
        Assert.That(result, Contains.Substring("Example field"));
        Assert.That(result, Contains.Substring("Example 1 class description"));
    }
}
