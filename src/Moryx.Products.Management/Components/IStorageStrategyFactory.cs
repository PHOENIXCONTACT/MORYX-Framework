// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
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
        IProductTypeStrategy CreateTypeStrategy(ProductTypeConfiguration config, CancellationToken cancellationToken);

        /// <summary>
        /// Create a new strategy instance
        /// </summary>
        IProductInstanceStrategy CreateInstanceStrategy(ProductInstanceConfiguration config, CancellationToken cancellationToken);

        /// <summary>
        /// Create a new strategy instance
        /// </summary>
        IProductLinkStrategy CreateLinkStrategy(ProductLinkConfiguration config, CancellationToken cancellationToken);

        /// <summary>
        /// Create a new strategy instance
        /// </summary>
        IProductRecipeStrategy CreateRecipeStrategy(ProductRecipeConfiguration config, CancellationToken cancellationToken);

        /// <summary>
        /// Destroy an instance
        /// </summary>
        void Destroy(object strategy);
    }
}
