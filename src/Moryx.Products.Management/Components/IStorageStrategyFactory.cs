// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Factory to instantiate <see cref="IProductTypeStrategy"/>
    /// </summary>
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface IStorageStrategyFactory
    {
        /// <summary>
        /// Create a new strategy instance
        /// </summary>
        Task<IProductTypeStrategy> CreateTypeStrategy(ProductTypeConfiguration config, CancellationToken cancellationToken);

        /// <summary>
        /// Create a new strategy instance
        /// </summary>
        Task<IProductInstanceStrategy> CreateInstanceStrategy(ProductInstanceConfiguration config, CancellationToken cancellationToken);

        /// <summary>
        /// Create a new strategy instance
        /// </summary>
        Task<IProductLinkStrategy> CreateLinkStrategy(ProductLinkConfiguration config, CancellationToken cancellationToken);

        /// <summary>
        /// Create a new strategy instance
        /// </summary>
        Task<IProductRecipeStrategy> CreateRecipeStrategy(ProductRecipeConfiguration config, CancellationToken cancellationToken);

        /// <summary>
        /// Destroy an instance
        /// </summary>
        void Destroy(object strategy);
    }
}
