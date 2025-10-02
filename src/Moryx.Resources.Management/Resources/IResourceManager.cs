// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.Modules;

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Major component managing the resource graph
    /// </summary>
    internal interface IResourceManager : IInitializablePlugin
    {
        /// <summary>
        /// Executes the intializer on this creator
        /// </summary>
        void ExecuteInitializer(IResourceInitializer initializer);

        /// <summary>
        /// Event raised when a resource was added at runtime
        /// </summary>
        event EventHandler<IResource> ResourceAdded;

        /// <summary>
        /// Event raised when a resource was removed at runtime
        /// </summary>
        event EventHandler<IResource> ResourceRemoved;

        /// <summary>
        /// Raised when the capabilities have changed.
        /// </summary>
        event EventHandler<ICapabilities> CapabilitiesChanged;

        /// <summary>
        /// Raised when a resource was changed during runtime (properties, collections or references)
        /// </summary>
        event EventHandler<IResource> ResourceChanged;
    }
}
