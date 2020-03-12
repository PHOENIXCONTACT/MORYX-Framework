// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Communication
{
    /// <summary>
    /// Interface for all generated serializers
    /// </summary>
    public interface IByteSerializer<THeader>
        where THeader : IBinaryHeader, new()
    {
        /// <summary>
        /// Serialize a message object into a binary message
        /// </summary>
        /// <param name="message">Message object</param>
        /// <returns>Serialized binary message</returns>
        BinaryMessage<THeader> Serialize(IBinaryRoot<THeader> message);

        /// <summary>
        /// Deserialize a binary message to an object
        /// </summary>
        /// <param name="message">Binary message received over TCP</param>
        /// <returns>Deserialized message</returns>
        IBinaryRoot<THeader> Deserialize(BinaryMessage<THeader> message);
    }
}
