// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using MQTTnet.Protocol;

namespace Moryx.AspNetCore.Mqtt.Components;

/// <summary>
/// Configuration for the MQTT Client
/// </summary>
[DataContract]
public class MqttConnectionConfig : ConfigBase
{
    /// <summary>
    /// Broker IP or URL
    /// </summary>
    [DataMember]
    [DefaultValue("localhost")]
    public string Host { set; get; }

    /// <summary>
    /// Broker port
    /// </summary>
    [DataMember]
    [DefaultValue(1883)]
    public int Port { set; get; }

    /// <summary>
    /// Id of the Hosted MQTT Client
    /// </summary>
    [DataMember]
    [DefaultValue("moryx-mqtt")]
    public string Id { set; get; }

    /// <summary>
    /// Root topic for the client
    /// </summary>
    [DataMember]
    [DefaultValue("")]
    public string RootTopic { set; get; } = string.Empty;

    /// <summary>
    /// Username to use for encrypted connection and communication with the MQTT Broker
    /// </summary>
    [DataMember]
    [DefaultValue("")]
    public string Username { set; get; } = string.Empty;

    /// <summary>
    /// Password to use for encrypted connection and communication with the MQTT Broker
    /// </summary>
    [DataMember]
    [DefaultValue("")]
    public string Password { set; get; } = string.Empty;

    /// <summary>
    /// Determines if the communication with the broker should be encrypted/secured.
    /// </summary>
    [DataMember]
    public bool Tls { set; get; }

    /// <summary>
    /// Mqtt quality of service for sent messages
    /// </summary>
    [DataMember]
    [DefaultValue(MqttQualityOfServiceLevel.ExactlyOnce)]
    public MqttQualityOfServiceLevel QoS { set; get; } = MqttQualityOfServiceLevel.ExactlyOnce;

    /// <summary>
    /// Waiting time before attempting to reconnect after a connection is lost
    /// </summary>
    [DataMember]
    [DefaultValue(30000)]
    public int ReconnectDelayMs { get; set; }

    /// <summary>
    /// Determines if the reconnection to the broker should be with a clean session 
    /// </summary>
    [DataMember]
    public bool ReconnectWithClientSession { get; set; }
}
