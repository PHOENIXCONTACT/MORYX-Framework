// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;
using Moryx.Drivers.Mqtt.Properties;
using System.Text.Json;
using System.Buffers;
using System.Text.Json.Serialization;

namespace Moryx.Drivers.Mqtt.MqttTopics
{
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

        private JsonSerializerOptions GetSystemTextJsonOptions()
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = Format == JsonFormat.CamelCase ? JsonNamingPolicy.CamelCase : null,
            };
            options.Converters.Clear();
            if (EnumsAsStrings)
            {
                options.Converters.Add(new JsonStringEnumConverter());
            }

            return options;
        }

    }
}

