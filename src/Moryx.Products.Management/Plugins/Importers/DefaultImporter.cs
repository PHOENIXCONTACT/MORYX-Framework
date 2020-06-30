// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.Products.Management.Importers
{
    [ExpectedConfig(typeof(DefaultImporterConfig))]
    [Plugin(LifeCycle.Transient, typeof(IProductImporter), Name = nameof(DefaultImporter))]
    public class DefaultImporter : ProductImporterBase<DefaultImporterConfig, DefaultImporterParameters>
    {
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

    public class DefaultImporterParameters : PrototypeParameters
    {
        [Required, PossibleTypes(typeof(ProductType))]
        public string ProductType { get; set; }
    }
}
