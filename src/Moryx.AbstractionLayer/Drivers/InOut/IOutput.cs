// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
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
        object Value { get; set;  }

        /// <summary>
        /// Index based output
        /// Only available for <see cref="SupportedAccess.Index"/>
        /// </summary>
        object this[int index] { get; set; }

        /// <summary>
        /// Key based output
        /// Only available for <see cref="SupportedAccess.Key"/>
        /// </summary>
        object this[string key] { get; set; }
    }
}
