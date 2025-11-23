// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers
{
    /// <summary>
    /// Classification of states from application point of view
    /// This classification is used to map driver states to general states.  It's a flags enum to allow multiple classifications for one state.
    /// The first 16 bits are reserved for general classification. Can be in specific driver implementations.
    /// </summary>
    [Flags]
    public enum StateClassification : int
    {
        /// <summary>
        /// Offline means not reachable
        /// </summary>
        Offline = 0,

        /// <summary>
        /// Initializing means preparing or starting
        /// </summary>
        Initializing = 1 << 0,

        /// <summary>
        /// Running means ready to work or working
        /// </summary>
        Running = 1 << 2,

        /// <summary>
        /// Busy means that the driver is running but is already in work
        /// </summary>
        Busy = 1 << 4,

        /// <summary>
        /// Maintenance means waiting for maintenance or maintenance running
        /// </summary>
        Maintenance = 1 << 6,

        /// <summary>
        /// Error means that is not running because there is an error
        /// </summary>
        Error = 1 << 8,
    }
}
