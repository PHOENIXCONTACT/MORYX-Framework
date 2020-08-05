// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.AbstractionLayer.Drivers.Message
{
    /// <summary>
    /// Multi-purpose driver that exchanges messages with the device
    /// </summary>
    public interface IMessageDriver<TMessage> : IDriver, IMessageCommunication<TMessage>
    {
        /// <summary>
        /// Access a named sub-channel of the driver
        /// </summary>
        IMessageCommunication<TMessage> this[string identifier] { get; }
    }
}