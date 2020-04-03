// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Products;
using Marvin.Container;
using Marvin.Modules;
using Marvin.Serialization;
using Marvin.Tools;

namespace Marvin.Products.Management.Importers
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
