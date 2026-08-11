// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Buffers;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Runtime.Serialization.DataContracts;
using System.Text.Json;
using System.Text.Json.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Drivers.Mqtt.Properties;
using Moryx.Serialization;

namespace Moryx.Drivers.Mqtt.Topics;

/// <summary>
/// MQTT Topic, where the published messages are in a JSON format
/// </summary>
[ResourceRegistration]
[Display(Name = nameof(Strings.MqttTopicJson_DisplayName), Description = nameof(Strings.MqttTopicJson_Description), ResourceType = typeof(Strings))]
public class MqttTopicJson : MqttTopic<object>
{
    /// <inheritdoc />
    [EntrySerialize]
    [DataMember]
    [Display(Name = nameof(Strings.MqttTopicIByteSerializable_MessageName), ResourceType = typeof(Strings))]
    public override string MessageName
    {
        get => base.MessageName;
        set => base.MessageName = value;
    }

    /// <summary>
    /// Format to use for the JSON of this topic
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.MqttTopicJson_Format), ResourceType = typeof(Strings))]
    public JsonFormat Format { get; set; }

        
    /// <summary>
    /// Controls if enums should be serialized as strings or as ints
    /// </summary>
    [DataMember, EntrySerialize]
    [Display(
        Name = nameof(Strings.MqttTopicJson_EnumsAsStrings),
        Description = nameof(Strings.MqttTopicJson_EnumsAsStrings_Description),
        ResourceType = typeof(Strings)
    )]
    public bool EnumsAsStrings { get; set; }

    /// <summary>
    /// Describes the default condition, under which fields should not be written to the serialized output
    /// </summary>
    [DataMember, EntrySerialize]
    [Display(
        Name = nameof(Strings.MqttTopicJson_IgnoreCondition),
        Description = nameof(Strings.MqttTopicJson_IgnoreCondition_Description),
        ResourceType = typeof(Strings)
    )]
    public JsonIgnoreCondition IgnoreCondition { get; set; }

    /// <summary>
    /// Encoder to use when encoding JsonContent
    /// </summary>
    [Display(
        Name = nameof(Strings.MqttTopicJson_EncoderOption),
        Description = nameof(Strings.MqttTopicJson_EncoderOption_Description),
        ResourceType = typeof(Strings)
    )]
    [DataMember, EntrySerialize]
    public JsonEncoderOption EncoderOption { get; set; }

    /// <inheritdoc />
    protected override byte[] Serialize(object payload)
    {
        var options = GetSystemTextJsonOptions();
        return JsonSerializer.SerializeToUtf8Bytes(payload, options);
    }

    /// <inheritdoc />
    protected override object Deserialize(ReadOnlySequence<byte> payload)
    {
        var options = GetSystemTextJsonOptions();
        return JsonDocument.Parse(payload).Deserialize(MessageType, options);
    }

    /// <summary>
    /// Get's the options for the JsonConverter to apply when serializing or deserializing each message.
    /// Overwriting this method allows customization that can't easily be done via the configuration properties
    /// like adding new JsonConverters, adding complex NaminPolicies and so on.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual JsonSerializerOptions GetSystemTextJsonOptions()
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = Format.ForSystemTextJson(),
            DefaultIgnoreCondition = IgnoreCondition.ForSystemTextJson(),
            Encoder = EncoderOption.ForSystemTextJson(),
        };
        options.Converters.Clear();
        if (EnumsAsStrings)
        {
            options.Converters.Add(new JsonStringEnumConverter());
        }

        return options;
    }

}
