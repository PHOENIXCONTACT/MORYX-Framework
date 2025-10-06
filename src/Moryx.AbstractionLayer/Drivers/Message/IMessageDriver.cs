// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Message
{
    /// <summary>
    /// Multi-purpose driver that exchanges information with a device
    /// </summary>
    public interface IMessageDriver<TMessage> : IDriver, IMessageChannel<TMessage>
    {
        /// <summary>
        /// Flag if the drivers supports identified channels or topics
        /// </summary>
        bool HasChannels { get; }

        /// <summary>
        /// Get channel using specialized API
        /// </summary>
        IMessageChannel<TChannel> Channel<TChannel>(string identifier);

        /// <summary>
        /// Get channel using specialized API
        /// </summary>
        IMessageChannel<TSend, TReceive> Channel<TSend, TReceive>(string identifier);
    }
}
