// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Drivers.Plc;

namespace Moryx.AbstractionLayer.Drivers.Message
{
    /// <summary>
    /// Interface for message based communication
    /// </summary>
    public interface IMessageCommunication
    {
        /// <summary>
        /// Reference to the underlying driver of this communication
        /// </summary>
        IMessageDriver Driver { get; }

        /// <summary>
        /// Identifier of this channel
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Send message through the driver
        /// </summary>
        void Send(object message);

        /// <summary>
        /// Event raised when the driver receives a message
        /// </summary>
        event EventHandler<object> Received;
    }
}