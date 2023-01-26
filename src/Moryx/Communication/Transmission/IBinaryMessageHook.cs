// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication
{
    /// <summary>
    /// To inform a benchmarking-tool about messages sent and received from the ControlSystem.
    /// </summary>
    public interface IBinaryMessageHook
    {
        /// <summary>
        /// Message of this type is being sent. Hooks my replace the object with another object
        /// </summary>
        /// <param name="payload">Payload that will be sent</param>
        /// <returns>True if the message shall be send, otherwise false</returns>
        bool SendingMessage(ref object payload);

        /// <summary>
        /// Message sent by the communicating component
        /// </summary>
        /// <param name="message">The binary message that was sent</param>
        void MessageSent(BinaryMessage message);

        /// <summary>
        /// Message received by the component
        /// </summary>
        /// <param name="message">The received binary message</param>
        void MessageReceived(BinaryMessage message);

        /// <summary>
        /// Deserialized typed message that was received
        /// </summary>
        /// <param name="payload">The payload that was read from the binary message. Hooks may replace this object</param>
        /// <returns>True if the message shall be published, otherwise false</returns>
        bool PublishingMessage(ref object payload);
    }
}
