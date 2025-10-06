// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.Communication;

namespace Moryx.Resources.Mqtt.Tests.TestMessages
{
    public class BoolMqttMessage : IIdentifierMessage, IByteSerializable
    {
        public byte[] ToBytes()
        {
            return new[] { Convert.ToByte(Message) };
        }

        public void FromBytes(byte[] bytes)
        {
            Message = BitConverter.ToBoolean(bytes, 0);
        }

        public string Identifier { get; set; }
        public bool Message { get; set; }
    }
}

