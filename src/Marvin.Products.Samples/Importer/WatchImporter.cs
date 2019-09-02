using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marvin.AbstractionLayer;
using Marvin.Container;
using Marvin.Modules;
using Marvin.Products.Management;
using Marvin.Products.Management.Importers;

namespace Marvin.Products.Samples
{
    [ExpectedConfig(typeof(WatchImporterConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IProductImporter), Name = nameof(WatchImporter))]
    public class WatchImporter : ProductImporterBase<WatchImporterConfig, SpecializedWatchImportParameters>
    {
        public IProductStorage Storage { get; set; }

        /// <summary>
        /// Import a product using given parameters
        /// </summary>
        protected override IProduct[] Import(SpecializedWatchImportParameters parameters)
        {
            var product = new WatchProduct
            {
                Name = parameters.Name,
                Identity = new ProductIdentity(parameters.Identifier, parameters.Revision),
                Watchface = new ProductPartLink<WatchfaceProduct>
                {
                    Product = (WatchfaceProduct)Storage.LoadProduct(new ProductIdentity(parameters.WatchfaceIdentifier, ProductIdentity.LatestRevision))
                },
                Needles = new List<NeedlePartLink>
                {
                    new NeedlePartLink
                    {
                        Role = NeedleRole.Minutes,
                        Product = (NeedleProduct)Storage.LoadProduct(new ProductIdentity(parameters.MinuteNeedleIdentifier, ProductIdentity.LatestRevision))
                    }
                }
            };

            return new IProduct[] { product };
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