// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Container;

namespace Moryx.Resources.Management
{
    /// <summary>
    /// Factory to create <see cref="IResourceInitializer"/>
    /// </summary>
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface IResourceInitializerFactory
    {
        /// <summary>
        /// Creates an <see cref="IResourceInitializer"/> with the given config
        /// </summary>
        Task<IResourceInitializer> Create(ResourceInitializerConfig config, CancellationToken cancellationToken);

        /// <summary>
        /// Destroys an <see cref="IResourceInitializer"/>
        /// </summary>
        void Destroy(IResourceInitializer resourceInitializer);
    }
}
