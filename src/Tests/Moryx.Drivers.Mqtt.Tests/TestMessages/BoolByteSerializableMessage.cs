// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Communication;

namespace Moryx.Drivers.Mqtt.Tests.TestMessages
{
    public class BoolByteSerializableMessage : IByteSerializable
    {
        public bool Message;
        public byte[] ToBytes()
        {
            return [Convert.ToByte(Message)];
        }

        public void FromBytes(byte[] bytes)
        {
            Message = BitConverter.ToBoolean(bytes, 0);
        }
    }

}

