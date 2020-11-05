// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Products.Management;
using Moryx.Products.Management.Importers;

namespace Moryx.Products.Samples
{
    [ExpectedConfig(typeof(WatchImporterConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IProductImporter), Name = nameof(WatchImporter))]
    public class WatchImporter : ProductImporterBase<WatchImporterConfig, SpecializedWatchImportParameters>
    {
        public IProductStorage Storage { get; set; }

        /// <summary>
        /// Import a product using given parameters
        /// </summary>
        protected override Task<ProductImporterResult> Import(ProductImportContext context, SpecializedWatchImportParameters parameters)
        {
            var product = new WatchType
            {
                Name = parameters.Name,
                Identity = new ProductIdentity(parameters.Identifier, parameters.Revision),
                Watchface = new ProductPartLink<WatchfaceTypeBase>
                {
                    Product = (WatchfaceType)Storage.LoadType(new ProductIdentity(parameters.WatchfaceIdentifier, ProductIdentity.LatestRevision))
                },
                Needles = new List<NeedlePartLink>
                {
                    new NeedlePartLink
                    {
                        Role = NeedleRole.Minutes,
                        Product = (NeedleType)Storage.LoadType(new ProductIdentity(parameters.MinuteNeedleIdentifier, ProductIdentity.LatestRevision))
                    }
                }
            };

            return Task.FromResult(new ProductImporterResult
            {
                ImportedTypes = new ProductType[] { product }
            });
        }
    }

    public class SpecializedWatchImportParameters : PrototypeParameters
    {
        [Required]
        public string WatchfaceIdentifier { get; set; }

        [Required]
        public string MinuteNeedleIdentifier { get; set; }
    }
}
