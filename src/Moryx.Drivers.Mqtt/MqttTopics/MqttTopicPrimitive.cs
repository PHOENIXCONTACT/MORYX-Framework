// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;
using Moryx.Drivers.Mqtt.Localizations;

namespace Moryx.Drivers.Mqtt.MqttTopics
{
    /// <summary>
    /// MQTT Topic, where primitive types are published
    /// </summary>
    [ResourceRegistration]
    [Display(Name = nameof(Strings.MQTT_TOPIC_PRIMITIVE), Description = nameof(Strings.MQTT_TOPIC_PRIMITIVE_DESCRIPTION), ResourceType = typeof(Localizations.Strings))]
    public class MqttTopicPrimitive : MqttTopic<IConvertible>
    {
        #region Properties

        /// <summary>
        /// This enum is used, to have a dropdown menu on the client ui including all primitive types for the MessageType 
        /// </summary>
        [EntrySerialize]
        [DataMember]
        [DisplayName("MessageName")]
        public TypeCode MessageNameEnum
        {
            get => Type.GetTypeCode(MessageType);
            set => MessageName = TypeCodeToString(value);
        }
        
        #endregion

        /// <inheritdoc />
        protected internal override byte[] Serialize(object payload)
        {
           if (payload is bool b)
                return BitConverter.GetBytes(b);
           if (payload is char c)
               return BitConverter.GetBytes(c);
           if (payload is double d)
               return BitConverter.GetBytes(d);
           if (payload is short s)
               return BitConverter.GetBytes(s);
           if (payload is int i)
               return BitConverter.GetBytes(i);
           if (payload is long l)
               return BitConverter.GetBytes(l);
           if (payload is float f)
               return BitConverter.GetBytes(f);
           if (payload is ushort us)
               return BitConverter.GetBytes(us);
           if (payload is uint ui)
               return BitConverter.GetBytes(ui);
           if (payload is ulong ul)
               return BitConverter.GetBytes(ul);
           if (payload is string str)
               return Encoding.ASCII.GetBytes(str);
           if (payload is byte byteMessage)
               return new[] {byteMessage};
           if (payload is Enum e)
           {
               throw new NotImplementedException();
           }
           if (payload is decimal dec)
           {
               throw new NotImplementedException();
           }
           if (payload is DateTime dt)
           {
               throw new NotImplementedException();
           }
           if (payload is DBNull dbNull)
           {
               throw new NotImplementedException();
           }
           if (payload is sbyte sByte)
           {
               throw new NotImplementedException();
           }
           throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected internal override IConvertible Deserialize(byte[] messageAsBytes)
        {
            if (MessageType == typeof(bool))
               return BitConverter.ToBoolean(messageAsBytes, 0);
            if (MessageType == typeof(char))
               return BitConverter.ToChar(messageAsBytes, 0);
            if (MessageType == typeof(double))
               return BitConverter.ToDouble(messageAsBytes, 0);
            if (MessageType == typeof(short))
               return BitConverter.ToInt16(messageAsBytes, 0);
            if (MessageType == typeof(int))
               return BitConverter.ToInt32(messageAsBytes, 0);
            if (MessageType == typeof(long))
               return BitConverter.ToInt64(messageAsBytes, 0);
            if (MessageType == typeof(ushort))
               return BitConverter.ToUInt16(messageAsBytes, 0);
            if (MessageType == typeof(uint))
               return BitConverter.ToUInt32(messageAsBytes, 0);
            if (MessageType == typeof(ulong))
               return BitConverter.ToUInt64(messageAsBytes, 0);
            if (MessageType == typeof(string))
               return Encoding.ASCII.GetString(messageAsBytes);
            if (MessageType == typeof(byte))
               return messageAsBytes[0];
            if (MessageType == typeof(float))
               return BitConverter.ToSingle(messageAsBytes, 0);
            throw new NotImplementedException();
        }

        #region private methodes
        private TypeCode StringToTypeCode(string messageName)
        {
            switch (messageName)
            {
                case "System.Boolean":
                    return TypeCode.Boolean;
                case "System.Byte":
                    return TypeCode.Byte;
                case "System.Char":
                    return TypeCode.Char;
                case "System.String":
                    return TypeCode.String;
                case "System.Double":
                    return TypeCode.Double;
                case "System.Single":
                    return TypeCode.Single;
                case "System.Int16":
                    return TypeCode.Int16;
                case "System.Int32":
                    return TypeCode.Int32;
                case "System.Int64":
                    return TypeCode.Int64;
                case "System.UInt16":
                    return TypeCode.UInt16;
                case "System.UInt32":
                    return TypeCode.UInt32;
                case "System.UInt64":
                    return TypeCode.UInt64;
                default:
                    return TypeCode.Empty;
            }
        }

        private string TypeCodeToString(TypeCode type)
        {
            switch (type)
            {
                case TypeCode.Boolean:
                    return "System.Boolean";
                case TypeCode.Byte:
                    return "System.Byte";
                case TypeCode.Char:
                    return "System.Char";
                case TypeCode.Double:
                    return "System.Double";
                case TypeCode.Single:
                    return "System.Single";
                case TypeCode.Int16:
                    return "System.Int16";
                case TypeCode.Int32:
                    return "System.Int32";
                case TypeCode.Int64:
                    return "System.Int64";
                case TypeCode.UInt16:
                    return "System.UInt16";
                case TypeCode.UInt32:
                    return "System.UInt32";
                case TypeCode.UInt64:
                    return "System.UInt64";
                case TypeCode.String:
                    return "System.String";
                default:
                    throw new NotImplementedException("This MessageType is not implemented");
            }
        }
        #endregion
    }
}

