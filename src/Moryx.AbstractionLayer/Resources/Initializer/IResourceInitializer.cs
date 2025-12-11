// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Resource initializers are used to create an initial set of resources.
    /// This will be used by the module console of the resource management.
    /// </summary>
    public interface IResourceInitializer : IAsyncConfiguredPlugin<ResourceInitializerConfig>
    {
        /// <summary>
        /// Name of this initializer
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Short description for this initializer
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Within this method, the resource trees should be created
        /// It is only necessary to return the roots
        /// </summary>
        Task<ResourceInitializerResult> ExecuteAsync(IResourceGraph graph, object parameters, CancellationToken cancellationToken = default);
    }
}
