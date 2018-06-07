using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marvin.AbstractionLayer;
using Marvin.Container;
using Marvin.Modules;
using Marvin.Products.Management;
using Marvin.Products.Management.Importers;

namespace Marvin.Products.Samples
{
    [ExpectedConfig(typeof(PrototypeImporterConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IProductImporter), Name = nameof(PrototypeImporter))]
    public class PrototypeImporter : ProductImporterBase<PrototypeImporterConfig, WatchImportParameters>
    {
        protected override WatchImportParameters Update(WatchImportParameters currentParameters)
        {
            if (string.IsNullOrEmpty(currentParameters.Name) && !string.IsNullOrEmpty(currentParameters.ProductType))
            {
                currentParameters.Name = currentParameters.ProductType + "_";
            }

            return currentParameters;
        }

        /// <summary>
        /// Import a product using given parameters
        /// </summary>
        protected override IProduct[] Import(WatchImportParameters parameters)
        {
            Product product = null;
            switch (parameters.ProductType)
            {
                case nameof(WatchfaceProduct):
                    product = new WatchfaceProduct
                    {
                        Numbers = new[] { 3, 6, 9, 12 }
                    };
                    break;
                case nameof(NeedleProduct):
                    product = new NeedleProduct();
                    break;
            }

            var identifier = parameters.Identifier;
            var rev = parameters.Revision;
            product.Identity = new ProductIdentity(identifier, rev);
            product.Name = parameters.Name;

            return new IProduct[] { product };
        }
    }

    public class WatchImportParameters : PrototypeParameters
    {
        [Required, TriggersUpdate]
        [PossibleProductValues(nameof(WatchfaceProduct), nameof(NeedleProduct))]
        public string ProductType { get; set; }
    }
}