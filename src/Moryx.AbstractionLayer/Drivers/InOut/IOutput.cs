// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.InOut
{
    /// <summary>
    /// Interface for writing output
    /// </summary>
    public interface IOutput
    {
        /// <summary>
        /// Access flags for the output
        /// </summary>
        SupportedAccess Access { get; }

        /// <summary>
        /// Single value output
        /// Only available for <see cref="SupportedAccess.Single"/>
        /// </summary>
        /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
        object Value { get; set; }

        /// <summary>
        /// Index based output
        /// Only available for <see cref="SupportedAccess.Index"/>
        /// </summary>
        /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
        object this[int index] { get; set; }

        /// <summary>
        /// Key based output
        /// Only available for <see cref="SupportedAccess.Key"/>
        /// </summary>
        /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
        object this[string key] { get; set; }
    }
}
