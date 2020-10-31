// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.Modules;

namespace Moryx.Products.Management.Importers
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
