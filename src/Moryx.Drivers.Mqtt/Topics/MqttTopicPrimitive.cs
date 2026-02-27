// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Buffers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Moryx.AbstractionLayer.Resources;
using Moryx.Drivers.Mqtt.Properties;
using Moryx.Serialization;

namespace Moryx.Drivers.Mqtt.Topics;

/// <summary>
/// MQTT Topic, where primitive types are published
/// </summary>
[ResourceRegistration]
[Display(Name = nameof(Strings.MqttTopicPrimitive_DisplayName), Description = nameof(Strings.MqttTopicPrimitive_Description), ResourceType = typeof(Strings))]
public class MqttTopicPrimitive : MqttTopic<IConvertible>
{
    /// <summary>
    /// Used to limit the amount of bytes we allocate on the stack before falling back to heap allocation
    /// </summary>
    private const int MAX_STACK_BUFFER_SIZE = 4096;
    #region Properties

    /// <summary>
    /// This enum is used, to have a dropdown menu on the client ui including all primitive types for the MessageType
    /// </summary>
    [EntrySerialize]
    [DataMember]
    [DisplayName("MessageName")]
    [DeniedValues(TypeCode.DateTime, TypeCode.DBNull, TypeCode.Decimal, TypeCode.Empty, TypeCode.Object, TypeCode.SByte)]
    public TypeCode MessageNameEnum
    {
        get => Type.GetTypeCode(MessageType);
        set => MessageName = TypeCodeToString(value);
    }
    #endregion

    /// <inheritdoc />
    protected override byte[] Serialize(object payload)
    {
        return payload switch
        {
            bool b => BitConverter.GetBytes(b),
            char c => BitConverter.GetBytes(c),
            float f => BitConverter.GetBytes(f),
            double d => BitConverter.GetBytes(d),
            short s => BitConverter.GetBytes(s),
            ushort us => BitConverter.GetBytes(us),
            int i => BitConverter.GetBytes(i),
            uint ui => BitConverter.GetBytes(ui),
            long l => BitConverter.GetBytes(l),
            ulong ul => BitConverter.GetBytes(ul),
            string str => Encoding.UTF8.GetBytes(str),
            byte byteMessage => [byteMessage],
            _ => throw new NotImplementedException(),
        };
    }

    private IConvertible DeserializeInternal(ReadOnlySpan<byte> messageSpan)
    {
        if (MessageType == typeof(bool))
        {
            return BitConverter.ToBoolean(messageSpan);
        }

        if (MessageType == typeof(char))
        {
            return BitConverter.ToChar(messageSpan);
        }

        if (MessageType == typeof(double))
        {
            return BitConverter.ToDouble(messageSpan);
        }

        if (MessageType == typeof(short))
        {
            return BitConverter.ToInt16(messageSpan);
        }

        if (MessageType == typeof(int))
        {
            return BitConverter.ToInt32(messageSpan);
        }

        if (MessageType == typeof(long))
        {
            return BitConverter.ToInt64(messageSpan);
        }

        if (MessageType == typeof(ushort))
        {
            return BitConverter.ToUInt16(messageSpan);
        }

        if (MessageType == typeof(uint))
        {
            return BitConverter.ToUInt32(messageSpan);
        }

        if (MessageType == typeof(ulong))
        {
            return BitConverter.ToUInt64(messageSpan);
        }

        if (MessageType == typeof(string))
        {
            return Encoding.UTF8.GetString(messageSpan);
        }

        if (MessageType == typeof(byte))
        {
            return messageSpan[0];
        }

        if (MessageType == typeof(float))
        {
            return BitConverter.ToSingle(messageSpan);
        }

        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected override IConvertible Deserialize(ReadOnlySequence<byte> payload)
    {
        if (payload.IsSingleSegment)
        {
            return DeserializeInternal(payload.FirstSpan);
        }
        else
        {
            var length = (int)payload.Length;
            var messageSpan = length < MAX_STACK_BUFFER_SIZE ? stackalloc byte[length] : new byte[length];
            payload.CopyTo(messageSpan);
            return DeserializeInternal(messageSpan);
        }
    }

    #region private methodes

    private static string TypeCodeToString(TypeCode type)
    {
        return type switch
        {
            TypeCode.Boolean => "System.Boolean",
            TypeCode.Byte => "System.Byte",
            TypeCode.Char => "System.Char",
            TypeCode.Double => "System.Double",
            TypeCode.Single => "System.Single",
            TypeCode.Int16 => "System.Int16",
            TypeCode.Int32 => "System.Int32",
            TypeCode.Int64 => "System.Int64",
            TypeCode.UInt16 => "System.UInt16",
            TypeCode.UInt32 => "System.UInt32",
            TypeCode.UInt64 => "System.UInt64",
            TypeCode.String => "System.String",
            _ => throw new NotImplementedException("This MessageType is not implemented"),
        };
    }
    #endregion
}
