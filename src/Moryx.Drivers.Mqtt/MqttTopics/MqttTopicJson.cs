// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Moryx.Drivers.Mqtt.Localizations;

namespace Moryx.Drivers.Mqtt.MqttTopics
{
    /// <summary>
    /// MQTT Topic, where the published messages are in a JSON format
    /// </summary>
    [ResourceRegistration]
    [Display(Name = nameof(Strings.MQTT_TOPIC_JSON), Description = nameof(Strings.MQTT_TOPIC_JSON_DESCRIPTION), ResourceType = typeof(Localizations.Strings))]
    public class MqttTopicJson : MqttTopic<object>
    {
        /// <inheritdoc />
        [EntrySerialize]
        [DataMember]
        [Display(Name = nameof(Strings.MESSAGE_NAME), ResourceType = typeof(Localizations.Strings))]
        public override string MessageName
        {
            get => base.MessageName;
            set => base.MessageName = value;
        }

        /// <summary>
        /// Format to use for the JSON of this topic
        /// </summary>
        [EntrySerialize, DataMember]
        [Display(Name = nameof(Strings.FORMAT), ResourceType = typeof(Localizations.Strings))]
        public JsonFormat Format { get; set; }

        /// <inheritdoc />
        protected internal override byte[] Serialize(object payload)
        {
            var json = JsonConvert.SerializeObject(payload, GetSettings());
            return Encoding.UTF8.GetBytes(json);
        }

        /// <inheritdoc />
        protected internal override object Deserialize(byte[] messageAsBytes)
        {
            var msg = Constructor();
            var json = Encoding.UTF8.GetString(messageAsBytes, 0, messageAsBytes.Length);
            JsonConvert.PopulateObject(json, msg, GetSettings());
            return msg;
        }

        private JsonSerializerSettings GetSettings()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
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

