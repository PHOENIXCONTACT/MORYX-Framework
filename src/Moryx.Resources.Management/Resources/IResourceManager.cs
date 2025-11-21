// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
        /// Executes the resource initializer
        /// </summary>
        Task<ResourceInitializerResult> ExecuteInitializer(IResourceInitializer initializer, object parameters);

        /// <summary>
        /// Executes a pre-configured the resource initializer selected by name
        /// </summary>
        Task<ResourceInitializerResult> ExecuteInitializer(string initializerName, object parameters);

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
    }
}
