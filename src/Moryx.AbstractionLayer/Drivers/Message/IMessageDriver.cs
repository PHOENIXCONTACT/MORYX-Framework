// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using System;

namespace Moryx.AbstractionLayer.Drivers.Message
{
    /// <summary>
    /// Multi-purpose driver that exchanges information with a device
    /// </summary>
    public interface IMessageDriver : IDriver
    {
        /// <summary>
        /// Send message through the driver
        /// </summary>
        void Send(object payload);

        /// <summary>
        /// Send data async through channel
        /// </summary>
        Task SendAsync(object payload);

        /// <summary>
        /// Event raised when the driver receives a message
        /// </summary>
        event EventHandler<object> Received;
    }
}
