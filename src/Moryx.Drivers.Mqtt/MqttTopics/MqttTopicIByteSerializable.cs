// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Communication;
using Moryx.Serialization;
using Moryx.Drivers.Mqtt.Localizations;
using Moryx.AbstractionLayer.Resources;
namespace Moryx.Drivers.Mqtt.MqttTopics;

/// <summary>
/// MQTT Topic, where the published messages implement IByteSerializable
/// </summary>
[ResourceRegistration]
[Display(Name = nameof(Strings.MQTT_TOPIC_BYTE), Description = nameof(Strings.MQTT_TOPIC_BYTE_DESCRIPTION), ResourceType = typeof(Localizations.Strings))]
public class MqttTopicIByteSerializable : MqttTopic<IByteSerializable>
{
    #region EntrySerialize

    /// <inheritdoc />
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.MESSAGE_NAME), ResourceType = typeof(Localizations.Strings))]
    [PossibleTypes(typeof(IByteSerializable))]
    public override string MessageName
    {
        get => base.MessageName;
        set => base.MessageName = value;
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    protected internal override byte[] Serialize(object payload)
    {
        return ((IByteSerializable)payload).ToBytes();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="messageAsBytes"></param>
    /// <returns></returns>
    protected internal override IByteSerializable Deserialize(byte[] messageAsBytes)
    {
        var msg = Constructor();
        msg.FromBytes(messageAsBytes);
        return msg;
    }
}

