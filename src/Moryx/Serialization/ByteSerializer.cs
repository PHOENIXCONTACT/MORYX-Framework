// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Communication;

namespace Moryx.Serialization
{
    /// <summary>
    /// Serializer implementation that can combine multiple protocol definitions
    /// </summary>
    public abstract class ByteSerializer<T> : IByteSerializer<T> where T : IBinaryHeader, new()
    {
        /// <summary>
        /// Serialize a message object into a binary message
        /// </summary>
        /// <param name="message">Message object</param>
        /// <returns>Serialized binary message</returns>
        BinaryMessage<T> IByteSerializer<T>.Serialize(IBinaryRoot<T> message)
        {
            return Serialize(message);
        }

        /// <summary>
        /// Serialize a message object into a binary message
        /// </summary>
        /// <param name="message">Message object</param>
        /// <returns>Serialized binary message</returns>
        public static BinaryMessage<T> Serialize(IBinaryRoot<T> message)
        {
            var header = message.Header;
            var payload = message.ToBytes();
            header.PayloadLength = payload.Length;

            return new BinaryMessage<T>(header, payload);
        }

        /// <summary>
        /// Deserialize a binary message to an object
        /// </summary>
        /// <param name="message">Binary message received over TCP</param>
        /// <returns>Deserialized message</returns>
        public abstract IBinaryRoot<T> Deserialize(BinaryMessage<T> message);
    }
}
