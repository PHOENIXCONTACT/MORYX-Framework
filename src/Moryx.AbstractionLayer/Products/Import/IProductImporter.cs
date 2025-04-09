// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Interface for plugins that can import products from file
    /// </summary>
    public interface IProductImporter : IConfiguredPlugin<ProductImporterConfig>
    {
        /// <summary>
        /// Name of the importer
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Importer is long running and synchronous execution useless
        /// </summary>
        bool LongRunning { get; }

        /// <summary>
        /// Get the parameters of this importer
        /// </summary>
        object Parameters { get; }

        /// <summary>
        /// Update parameters based on partial input
        /// </summary>
        object Update(object currentParameters);

        /// <summary>
        /// Import products using given parameters
        /// </summary>
        Task<ProductImporterResult> Import(ProductImportContext context, object parameters);
    }
}
