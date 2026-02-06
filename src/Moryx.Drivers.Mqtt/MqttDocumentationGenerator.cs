// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Drivers.Mqtt.Properties;
using Moryx.Drivers.Mqtt.Topics;
using Moryx.Serialization;

namespace Moryx.Drivers.Mqtt;

/// <summary>
/// Allows automatically creating and updating a description of the published and subscribed topics of a connected MqttDriver
/// </summary>
[ResourceRegistration]
[Display(Name = nameof(Strings.MqttDocumentationGenerator_DisplayName), Description = nameof(Strings.MqttDocumentationGenerator_Description), ResourceType = typeof(Strings))]
public class MqttDocumentationGenerator : Resource
{
    /// <summary>
    /// MqttDriver to collect topics from
    /// </summary>
    [ResourceReference(ResourceRelationType.Decorated)]
    public MqttDriver MqttDriver { get; set; }

    /// <summary>
    /// Check when a file should be generated each time this resource is started
    /// </summary>
    [DataMember, EntrySerialize]
    [Display(Name = nameof(Strings.MqttDocumentationGenerator_AutoGenerateOnStart), ResourceType = typeof(Strings))]
    public bool AutoGenerateOnStart { get; set; }

    /// <summary>
    /// OutputPath used when documentation is generated on startup
    /// </summary>
    [Display(Name = nameof(Strings.MqttDocumentationGenerator_OutputPath), ResourceType = typeof(Strings))]
    [DataMember, EntrySerialize, DefaultValue("./Schema.md")]
    public string OutputPath { get; set; } = "./Schema.md";

    /// <summary>
    /// Generates the documentation file and writes it to the specified path;
    /// </summary>
    /// <param name="schemaPath"></param>
    [EntrySerialize]
    [Display(Name = nameof(Strings.MqttDocumentationGenerator_GenerateDocumentationFile), Description = nameof(Strings.MqttDocumentationGenerator_GenerateDocumentationFile_Description), ResourceType = typeof(Strings))]
    public void GenerateDocumentationFile(string schemaPath)
    {
        if (string.IsNullOrWhiteSpace(schemaPath))
        {
            schemaPath = OutputPath;
        }

        var file = File.Open(schemaPath, FileMode.Create);
        using var writer = new StreamWriter(file, Encoding.UTF8);
        GenerateDocumentation(writer);
    }

    /// <summary>
    /// Generates the Documentation and writes it to the provided stream
    /// </summary>
    /// <param name="writer"></param>
    public void GenerateDocumentation(TextWriter writer)
    {
        writer.WriteLine("# Topic Structure");

        writer.WriteLine("You may need a mermaid viewer to visualize. I recommend the 'Markown Preview Mermaid Support' extension for vs code");

        writer.WriteLine("```mermaid");
        GenerateMermaidGraph(writer);
        writer.WriteLine("```");
        writer.WriteLine();

        writer.WriteLine("# Message definitions");
        writer.WriteLine();
        foreach (var topic in MqttDriver.Channels)
        {
            try
            {
                RenderTopicDoc(writer, topic);
            }
            catch (Exception)
            {
                writer.WriteLine($"Failed to render topic: {topic.Identifier}");
            }
        }
    }

    /// <inheritdoc />
    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        await base.OnStartAsync(cancellationToken);
        if (!AutoGenerateOnStart)
        {
            return;
        }
        if (MqttDriver is null)
        {
            Logger.LogWarning("MqttDocumentationGenerator not connected to a MqttDriver");
            return;
        }
        try
        {
            GenerateDocumentationFile(OutputPath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to write schema file to {file}", OutputPath);
        }
    }

    private static void GenerateSchemaSerializationOptions(bool enumsAsString, out JsonSerializerOptions options, out JsonSchemaExporterOptions exporterOptions)
    {
        options = new JsonSerializerOptions(JsonSerializerDefaults.General);
        options.WriteIndented = true;
        options.RespectNullableAnnotations = true;
        options.TypeInfoResolver = new DefaultJsonTypeInfoResolver();

        options.Converters.Clear();
        if (enumsAsString)
        {
            options.Converters.Add(new JsonStringEnumConverter());
            exporterOptions = new JsonSchemaExporterOptions()
            {
                TreatNullObliviousAsNonNullable = true,
            };
        }
        else
        {
            exporterOptions = new JsonSchemaExporterOptions()
            {
                TreatNullObliviousAsNonNullable = true,
                // Without the TransformSchemaNode integer enums would just be rendered as type integer
                TransformSchemaNode = (context, schema) =>
                {
                    if (schema is not JsonObject obj)
                    {
                        obj = new JsonObject();
                    }
                    var provider =
                        context.PropertyInfo?.AttributeProvider
                        ?? context.TypeInfo.Type;

                    var descriptionAttr = provider?
                        .GetCustomAttributes(inherit: true)
                        .OfType<DescriptionAttribute>()
                        .FirstOrDefault();

                    if (descriptionAttr != null)
                    {
                        obj["description"] = descriptionAttr.Description;
                    }

                    if (context.TypeInfo.Type.IsEnum)
                    {
                        var enumValues =
                            Enum.GetValues(context.TypeInfo.Type)
                            .Cast<object>()
                            .Select(v => JsonValue.Create((int)v))
                            .ToArray();
                        var enumNames = Enum.GetNames(context.TypeInfo.Type).Select(v => JsonValue.Create(v)).ToArray();
                        var enumArray = new JsonArray(enumValues);
                        obj["enum"] = enumArray;
                        obj["x-enumNames"] = new JsonArray(enumNames);
                    }
                    return obj;
                }
            };
        }
    }

    private static void WriteJsonSchemaForType(TextWriter writer, Type messageType, bool enumsAsString)
    {
        GenerateSchemaSerializationOptions(enumsAsString, out var options, out var exporterOptions);
        writer.WriteLine(options.GetJsonSchemaAsNode(messageType, exporterOptions).ToString());
    }

    private void GenerateMermaidGraph(TextWriter writer)
    {
        writer.WriteLine("""
        ---
        config:
            theme: 'base'
            themeVariables:
                primaryTextColor: '#000'
                darkMode: true
                background: rgb(0, 0, 0)
        ---
        """);
        // TODO: Consider make styling configurable
        writer.WriteLine("graph TD;");
        writer.WriteLine("classDef receiveRetain fill:#DAE8FC");
        writer.WriteLine("classDef receive fill:#F8CECC");
        writer.WriteLine("classDef publishRetain fill:#D5E8D4");
        writer.WriteLine("classDef publish fill:#E1D5E7");
        writer.WriteLine("classDef bidirectionalRetain fill:#bbf");
        writer.WriteLine("classDef bidirectional fill:#bbf");
        var idCounter = 1;
        Stack<(string topic, int id)> previousTopics = new();
        foreach (var topic in MqttDriver.Channels.OrderBy(t => t.Identifier))
        {
            var topicString = MqttDriver.Identifier + topic.Identifier;

            (string topic, int id) previous;
            while (previousTopics.TryPeek(out previous) && !topicString.StartsWith(previous.topic + '/'))
            {
                previousTopics.Pop();
            }

            var segments = topicString.Substring(previous.topic?.Length + 1 ?? 0).Split("/");
            foreach (var segment in segments)
            {
                var segmentName = segment;//.Replace("{", @"\{").Replace("}", @"\}");
                var newId = idCounter++;
                if (previous.topic == null)
                {
                    writer.WriteLine($"A{newId}[\"{segmentName}\"];");
                    previous = (segment, newId);
                }
                else
                {
                    writer.WriteLine($"A{previous.id} --> A{newId}[\"{segmentName}\"];");
                    previous = (previous.topic + '/' + segment, newId);
                }
                previousTopics.Push(previous);
            }

            var nodeClass = topic.TopicType switch
            {
                TopicType.BiDirectional => "bidirectional",
                TopicType.SubscribeOnly => "receive",
                TopicType.PublishOnly => "publish",
                _ => throw new NotImplementedException(),
            };

            writer.WriteLine($"class A{previous.id} {nodeClass}{(topic.Retain ? "Retain" : "")}");
            // anchor links don't work in gitlab. See: [gitlab issue 405296](https://gitlab.com/gitlab-org/gitlab/-/issues/405296)
            // so I removed them for now
            // var topicLink = topicString.ToLower().Replace("/", "").Replace("{", "").Replace("}", "");
            // writer.WriteLine($"click A{previous.id} #{topicLink}");
        }
    }

    private void RenderTopicDoc(TextWriter writer, MqttTopic topic)
    {
        writer.Write("## ");
        if (topic.Identifier is null)
        {
            writer.WriteLine("null topic");
        }
        else
        {
            writer.WriteLine(MqttDriver.Identifier + topic.Identifier);
        }
        writer.WriteLine();

        if (topic.MessageType is not Type messageType)
        {

            writer.Write("Unresolved topic type");
            writer.WriteLine(topic.MessageName);
            writer.WriteLine();
            return;
        }

        // TODO: support more topic types if necessary (strategy to allow rendering of custom topics?)
        switch (topic)
        {
            case MqttTopicJson jsonTopic:
                GenerateJsonTopicDoc(writer, jsonTopic, messageType);
                break;
            default:
                writer.WriteLine("Topic of type {0} and message type {1}", topic.GetType().Name, messageType.Name);
                break;
        }
        writer.WriteLine();
    }

    private void GenerateJsonTopicDoc(TextWriter writer, MqttTopicJson topic, Type messageType)
    {
        var enumsAsString = topic is MqttTopicJson tJson && tJson.EnumsAsStrings;

        writer.WriteLine("```jsonc");
        writer.WriteLine("// {0}", messageType.FullName);
        writer.WriteLine("// kind: {0}", topic.TopicType.ToString());
        writer.WriteLine("// retain: {0}", topic.Retain);
        try
        {
            WriteJsonSchemaForType(writer, messageType, enumsAsString);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to generate schema for {type}", messageType);
            writer.WriteLine("// Failed to generate schema");
        }
        writer.WriteLine("```");
    }
}
