// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Interface for resources that are visible outside of the abstraction layer
    /// </summary>
    public interface IPublicResource : IResource
    {
        /// <summary>
        /// The resource's capabilities 
        /// </summary>
        ICapabilities Capabilities { get; }

        /// <summary>
        /// Raised when the capabilities have changed.
        /// </summary>
        event EventHandler<ICapabilities> CapabilitiesChanged;
    }
}
