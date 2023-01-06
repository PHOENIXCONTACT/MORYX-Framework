// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.AbstractionLayer.Drivers
{
    /// <summary>
    /// Interface for all device drivers
    /// </summary>
    public interface IDriver : IResource
    {
        /// <summary>
        /// Current state of the device
        /// </summary>
        IDriverState CurrentState { get; }

        /// <summary>
        /// Event raised when the device state changed
        /// </summary>
        event EventHandler<IDriverState> StateChanged;
    }
}
