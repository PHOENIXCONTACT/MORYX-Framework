// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Buffers;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Communication;
using Moryx.Drivers.Mqtt.Properties;
using Moryx.Serialization;

namespace Moryx.Drivers.Mqtt.Topics;

/// <summary>
/// MQTT Topic, where the published messages implement IByteSerializable
/// </summary>
[ResourceRegistration]
[Display(Name = nameof(Strings.MqttTopicIByteSerializable_DisplayName), Description = nameof(Strings.MqttTopicIByteSerializable_Description), ResourceType = typeof(Strings))]
public class MqttTopicIByteSerializable : MqttTopic<IByteSerializable>
{
    #region EntrySerialize

    /// <inheritdoc />
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.MqttTopicIByteSerializable_MessageName), ResourceType = typeof(Strings))]
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
    protected override byte[] Serialize(object payload)
    {
        return ((IByteSerializable)payload).ToBytes();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    protected override IByteSerializable Deserialize(ReadOnlySequence<byte> payload)
    {
        var msg = Constructor();
        msg.FromBytes(payload.ToArray());
        return msg;
    }
}

