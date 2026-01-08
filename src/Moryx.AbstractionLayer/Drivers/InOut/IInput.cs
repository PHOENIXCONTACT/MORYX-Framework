// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.InOut
{
    /// <summary>
    /// Interface for reading input
    /// </summary>
    public interface IInput
    {
        /// <summary>
        /// Access flags for the input
        /// </summary>
        SupportedAccess Access { get; }

        /// <summary>
        /// Single value input
        /// Only available for <see cref="SupportedAccess.Single"/>
        /// </summary>
        /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
        object Value { get; }

        /// <summary>
        /// Index based input
        /// Only available for <see cref="SupportedAccess.Index"/>
        /// </summary>
        /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
        object this[int index] { get; }

        /// <summary>
        /// Key based input
        /// Only available for <see cref="SupportedAccess.Key"/>
        /// </summary>
        /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
        object this[string key] { get; }

        /// <summary>
        /// Event raised when any input value changed
        /// </summary>
        event EventHandler<InputChangedEventArgs> InputChanged;
    }
}
