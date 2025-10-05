// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Communication;

namespace Moryx.Resources.Mqtt.Tests.TestMessages
{
    public class BoolByteSerializableMessage : IByteSerializable
    {
        public bool Message;
        public byte[] ToBytes()
        {
            return new[] { Convert.ToByte(Message) };
        }

        public void FromBytes(byte[] bytes)
        {
            Message = BitConverter.ToBoolean(bytes, 0);
        }
    }

}

