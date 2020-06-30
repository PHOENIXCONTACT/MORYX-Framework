// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Products;
using Marvin.Modules;

namespace Marvin.Products.Management.Importers
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
        /// Get the parameters of this importer
        /// </summary>
        IImportParameters Parameters { get; }

        /// <summary>
        /// Update parameters based on partial input
        /// </summary>
        IImportParameters Update(IImportParameters currentParameters);

        /// <summary>
        /// Import products using given parameters
        /// </summary>
        IProductType[] Import(IImportParameters parameters);
    }
}
