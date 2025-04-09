// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication
{
    /// <summary>
    /// Result or processing the byte stream
    /// </summary>
    public enum ByteReadResult
    {
        /// <summary>
        /// Byte stream was read successfully and is still open
        /// </summary>
        KeepReading,
        /// <summary>
        /// Message or message fragement invalid
        /// </summary>
        Failure
    }

    /// <summary>
    /// Validator instance that checks of parsed headers match this listeners requirements
    /// </summary>
    public interface IMessageInterpreter : IEquatable<IMessageInterpreter>
    {
        /// <summary>
        /// Create transmission for first communication
        /// </summary>
        /// <returns></returns>
        IReadContext CreateContext();

        /// <summary>
        /// Serialize the message into its binary representation
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        byte[] SerializeMessage(BinaryMessage message);

        /// <summary>
        /// Method called by the connection once the bytes were read
        /// </summary>
        /// <param name="context">Current transmission</param>
        /// <param name="readBytes">Bytes that were read from the connection</param>
        /// <param name="publishCompleteMessage">Delegate to publish a complete message read from the byte stream</param>
        ByteReadResult ProcessReadBytes(IReadContext context, int readBytes, Action<BinaryMessage> publishCompleteMessage);

        /// <summary>
        /// Create error response for a faulty message
        /// </summary>
        bool ErrorResponse(IReadContext context, out byte[] lastWill);
    }
}
