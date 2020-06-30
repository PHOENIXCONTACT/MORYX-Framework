// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication
{
    /// <summary>
    /// Interaface for binary messages
    /// </summary>
    public class BinaryMessage
    {
        /// <summary>
        /// Empty payload to optimize memory usage and avoid creating unnecessary objects
        /// </summary>
        public static byte[] EmptyBytes = new byte[0];

        /// <summary>
        /// Default constructor used to create a <see cref="BinaryMessage"/>
        /// </summary>
        public BinaryMessage()
        {
        }

        /// <summary>
        /// Constructor to create a binary message with a payload byte array
        /// </summary>
        /// <param name="payload">Payload byte array for this binary message</param>
        public BinaryMessage(byte[] payload)
        {
            Payload = payload;
        }

        /// <summary>
        /// Message payload
        /// </summary>
        public byte[] Payload { get; set; }
    }

    /// <summary>
    /// Interaface for binary messages with generic header
    /// </summary>
    public class BinaryMessage<THeader> : BinaryMessage
        where THeader : IBinaryHeader, new()
    {
        /// <summary>
        /// Default constructor used to create a <see cref="BinaryMessage"/>
        /// </summary>
        public BinaryMessage()
        {
        }

        /// <summary>
        /// Constructor to create a binary message with a payload byte array
        /// </summary>
        /// <param name="header">Header of this message</param>
        /// <param name="payload">Payload byte array for this binary message</param>
        public BinaryMessage(THeader header, byte[] payload) : base(payload)
        {
            Header = header;
        }

        /// <summary>
        /// Header of this message
        /// </summary>
        public THeader Header { get; set; }
    }
}
