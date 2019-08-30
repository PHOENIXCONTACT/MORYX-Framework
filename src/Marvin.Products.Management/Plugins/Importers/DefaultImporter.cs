using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Marvin.AbstractionLayer;
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
        protected override IProduct[] Import(DefaultImporterParameters parameters)
        {
            // TODO: Use type wrapper
            var type = ReflectionTool.GetPublicClasses<Product>(p => p.Name == parameters.ProductType)
                .First();

            var productType = (Product)Activator.CreateInstance(type);
            productType.Identity = new ProductIdentity(parameters.Identifier, parameters.Revision);
            productType.Name = parameters.Name;

            return new IProduct[] { productType };
        }
    }

    public class DefaultImporterParameters : PrototypeParameters
    {
        [Required, PossibleTypes(typeof(Product))]
        public string ProductType { get; set; }
    }
}