// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.AbstractionLayer.Drivers.Rfid
{
    /// <summary>
    /// Driver API of the RFID driver
    /// </summary>
    public interface IRfidDriver : IDriver
    {
        /// <summary>
        /// Event raised when tags are read by the antenna
        /// </summary>
        event EventHandler<RfidTag[]> TagsRead;
    }
}
