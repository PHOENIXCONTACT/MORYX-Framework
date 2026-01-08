// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Basic interface of a Resource.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Id of the resource
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Name of this resource instance
        /// </summary>
        string Name { get; }

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
