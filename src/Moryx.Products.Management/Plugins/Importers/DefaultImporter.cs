// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.Products.Management.Importers
{
    /// <summary>
    /// Product importer which can create all types of products
    /// </summary>
    [ExpectedConfig(typeof(ProductImporterConfig))]
    [Plugin(LifeCycle.Transient, typeof(IProductImporter), Name = nameof(DefaultImporter))]
    public class DefaultImporter : ProductImporterBase<ProductImporterConfig, DefaultImporterParameters>
    {
        /// <inheritdoc />
        protected override IProductType[] Import(DefaultImporterParameters parameters)
        {
            // TODO: Use type wrapper
            var type = ReflectionTool.GetPublicClasses<ProductType>(p => p.Name == parameters.ProductType)
                .First();

            var productType = (ProductType)Activator.CreateInstance(type);
            productType.Identity = new ProductIdentity(parameters.Identifier, parameters.Revision);
            productType.Name = parameters.Name;

            return new IProductType[] { productType };
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
