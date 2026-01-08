// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Communication.Sockets
{
    internal class InvalidHeaderException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public InvalidHeaderException()
        {
        }

        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        /// <param name="si">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public InvalidHeaderException(SerializationInfo si, StreamingContext context)
            : base(si, context)
        {
        }

        public InvalidHeaderException(string message)
            : base(message)
        {
        }
    }
}
