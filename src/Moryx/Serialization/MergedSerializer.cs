// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using Moryx.Communication;

namespace Moryx.Serialization
{
    /// <summary>
    /// Helper class to build <see cref="MergedSerializer.Generic{T}"/>
    /// </summary>
    public static class MergedSerializer
    {
        /// <summary>
        /// Extension method to extend a serializer with additional serializers for type support
        /// </summary>
        /// <typeparam name="T">Type of header</typeparam>
        /// <param name="coreSerializer">Current serializer</param>
        /// <param name="extension">Additional serializer</param>
        /// <returns>Merged instance</returns>
        public static IByteSerializer<T> Extend<T>(this IByteSerializer<T> coreSerializer, IByteSerializer<T> extension)
            where T : IBinaryHeader, new()
        {
            return (coreSerializer as Generic<T> ?? new Generic<T>(coreSerializer)).Add(extension);
        }

        /// <summary>
        /// Serializer implementation that can merge several protocol specifications into a single serializer
        /// </summary>
        private sealed class Generic<T> : ByteSerializer<T>
            where T : IBinaryHeader, new()
        {
            /// <summary>
            /// Chain of responsibility of serializers
            /// </summary>
            private readonly ConcurrentBag<IByteSerializer<T>> _serializers = [];

            /// <summary>
            /// Initialize merged serializer with a minimum of one target
            /// </summary>
            /// <param name="core">Serializer for the core protocol specification</param>
            internal Generic(IByteSerializer<T> core)
            {
                _serializers.Add(core);
            }

            /// <summary>
            /// Extend base serializer with additional messages
            /// </summary>
            public Generic<T> Add(IByteSerializer<T> additionalSerializer)
            {
                _serializers.Add(additionalSerializer);
                return this;
            }

            /// <summary>
            /// Deserialize a binary message to an object
            /// </summary>
            /// <param name="message">Binary message received over TCP</param>
            /// <returns>Deserialized message</returns>
            public override IBinaryRoot<T> Deserialize(BinaryMessage<T> message)
            {
                // Check if any of the serializers can handle the message
                return _serializers.Select(serializer => serializer.Deserialize(message)).FirstOrDefault(payload => payload != null);
            }
        }
    }
}
