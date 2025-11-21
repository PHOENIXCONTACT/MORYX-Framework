// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Moryx.AbstractionLayer.Products;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.Products.Management.Importers
{
    /// <summary>
    /// Product importer which can create all types of products
    /// </summary>
    [ExpectedConfig(typeof(ProductImporterConfig))]
    [ProductImporter(nameof(DefaultImporter))]
    public class DefaultImporter : ProductImporterBase<ProductImporterConfig, DefaultImporterParameters>
    {
        /// <inheritdoc />
        protected override Task<ProductImporterResult> Import(ProductImportContext context, DefaultImporterParameters parameters)
        {
            var productType = (ProductType)TypeTool.CreateInstance<ProductType>(parameters.ProductType);
            productType.Identity = new ProductIdentity(parameters.Identifier, parameters.Revision);
            productType.Name = parameters.Name;

            return Task.FromResult(new ProductImporterResult
            {
                ImportedTypes = [productType]
            });
        }
    }

    /// <summary>
    /// Parameters for the default importer
    /// </summary>
    public class DefaultImporterParameters : PrototypeParameters
    {
        /// <summary>
        /// Product type to import
        /// </summary>
        [DisplayName("Product type"), Description("Type of product to import")]
        [Required, PossibleTypes(typeof(ProductType))]
        public string ProductType { get; set; }
    }
}
