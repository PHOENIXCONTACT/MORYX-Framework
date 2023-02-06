// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Container;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Factory to create configured importers
    /// </summary>
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface IProductImporterFactory
    {
        /// <summary>
        /// Create new importer
        /// </summary>
        IProductImporter Create(ProductImporterConfig config);

        /// <summary>
        /// Destroy an importer
        /// </summary>
        void Destroy(IProductImporter instance);
    }
}
