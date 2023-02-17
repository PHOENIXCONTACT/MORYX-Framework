// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
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
        IResourceInitializer Create(ResourceInitializerConfig config);

        /// <summary>
        /// Destroys an <see cref="IResourceInitializer"/>
        /// </summary>
        void Destroy(IResourceInitializer resourceInitializer);
    }
}
