// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Moryx.Drivers.Mqtt.Properties;
using System;
using System.Text.Json;

namespace Moryx.Drivers.Mqtt.MqttTopics
{

    public enum SerializerSelection
    {
        NewtonsoftJson,
        SystemTextJson
    }
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

        [DataMember, EntrySerialize]
        // TODO: Add localized display attribute
        public SerializerSelection Serializer { get; set; }

        [DataMember, EntrySerialize]
        public bool EnumsAsStrings { get; set; }

        /// <inheritdoc />
        protected internal override byte[] Serialize(object payload)
        {
            if (Serializer == SerializerSelection.SystemTextJson)
            {

                var options = GetSystemTextJsonOptions();
                return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(payload, options);
            }
            var json = JsonConvert.SerializeObject(payload, GetSettings());
            return Encoding.UTF8.GetBytes(json);
        }

        /// <inheritdoc />
        protected internal override object Deserialize(ArraySegment<byte> messageAsBytes)
        {
            if (Serializer == SerializerSelection.SystemTextJson)
            {
                var options = GetSystemTextJsonOptions();
                return System.Text.Json.JsonSerializer.Deserialize(messageAsBytes, MessageType, options);
            }

            var msg = Constructor();
            var json = Encoding.UTF8.GetString(messageAsBytes.AsSpan()); // TODO: consider moving to system.text.json. This would make creating a temporary string unnecessary
            JsonConvert.PopulateObject(json, msg, GetSettings());
            return msg;
        }

        private JsonSerializerOptions GetSystemTextJsonOptions()
        {
            var options = new System.Text.Json.JsonSerializerOptions()
            {
                PropertyNamingPolicy = Format == JsonFormat.camelCase ? JsonNamingPolicy.CamelCase : null,
            };
            options.Converters.Clear();
            if (EnumsAsStrings)
                options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            return options;
        }

        private JsonSerializerSettings GetSettings()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            if (EnumsAsStrings)
                settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            if (Format == JsonFormat.camelCase)
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return settings;
        }

    }

    /// <summary>
    /// Configuration value for the JSON format
    /// </summary>
    public enum JsonFormat
    {
        PascalCase = 0,
        camelCase = 1
    }
}

