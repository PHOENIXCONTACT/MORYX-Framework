// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.InOut
{
    /// <summary>
    /// Interface for reading input
    /// </summary>
    public interface IInput<out TIn>
    {
        /// <summary>
        /// Access flags for the input
        /// </summary>
        SupportedAccess Access { get; }

        /// <summary>
        /// Single value input
        /// Only available for <see cref="SupportedAccess.Single"/>
        /// </summary>
        /// <exception cref="DriverStateException">Will be thrown when the driver is in wrong state</exception>
        TIn Value { get; }

        /// <summary>
        /// Index based input
        /// Only available for <see cref="SupportedAccess.Index"/>
        /// </summary>
        /// <exception cref="DriverStateException">Will be thrown when the driver is in wrong state</exception>
        TIn this[int index] { get; }

        /// <summary>
        /// Key based input
        /// Only available for <see cref="SupportedAccess.Key"/>
        /// </summary>
        /// <exception cref="DriverStateException">Will be thrown when the driver is in wrong state</exception>
        TIn this[string key] { get; }

        /// <summary>
        /// Event raised when any input value changed
        /// </summary>
        event EventHandler<InputChangedEventArgs> InputChanged;
    }
}
