// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Message
{
    /// <summary>
    /// Multipurpose driver that exchanges information with a device
    /// </summary>
    public interface IMessageDriver : IDriver, IMessageChannel
    {
        /// <summary>
        /// Flag if the drivers supports identified channels or topics
        /// </summary>
        bool HasChannels { get; }

        /// <summary>
        /// Get channel using specialized API
        /// </summary>
        IMessageChannel Channel(string identifier);
    }
}
