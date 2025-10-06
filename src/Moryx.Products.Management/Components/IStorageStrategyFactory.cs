// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Factory to instantiate <see cref="IProductTypeStrategy"/>
    /// </summary>
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    public interface IStorageStrategyFactory
    {
        /// <summary>
        /// Create a new strategy instance
        /// </summary>
        IProductTypeStrategy CreateTypeStrategy(ProductTypeConfiguration config);

        /// <summary>
        /// Create a new strategy instance
        /// </summary>
        IProductInstanceStrategy CreateInstanceStrategy(ProductInstanceConfiguration config);

        /// <summary>
        /// Create a new strategy instance
        /// </summary>
        IProductLinkStrategy CreateLinkStrategy(ProductLinkConfiguration config);

        /// <summary>
        /// Create a new strategy instance
        /// </summary>
        IProductRecipeStrategy CreateRecipeStrategy(ProductRecipeConfiguration config);

        /// <summary>
        /// Destroy an instance
        /// </summary>
        void Destroy(object strategy);
    }
}
