using Moryx.AbstractionLayer.Products;
using Moryx.Modules;
using Moryx.Products.Model;
using System;
using System.Collections.Generic;

namespace Moryx.Products.Management.Implementation.Storage
{
    internal class ConstructorStrategyInformation<TObject,TConfiguration, TStrategy> where TConfiguration : IPluginConfig where TStrategy: IConfiguredPlugin<TConfiguration>
    {
        public string Identifier { get; set; }
        public Func<TObject> Constructor { get; set; }
        public TStrategy Strategy { get; set; }
    }

    internal class ProductTypeInformation : ConstructorStrategyInformation<ProductType, ProductTypeConfiguration, IProductTypeStrategy>
    {
        public IProductInstanceStrategy InstanceStrategy { get; set; }

        public ProductType CreateTypeFromEntity(ProductTypeEntity entity)
        {
            var productType = Constructor();
            productType.Name = entity.Name;
            productType.Id = entity.Id;
            productType.Identity = new ProductIdentity(entity.Identifier, entity.Revision);

            ProductInstance v;
            
            return productType;
        }

        public IDictionary<string,ConstructorStrategyInformation<IProductPartLink, ProductLinkConfiguration, IProductLinkStrategy>> PartLinksInformation =
            new Dictionary<string,ConstructorStrategyInformation<IProductPartLink, ProductLinkConfiguration, IProductLinkStrategy>>();

    
    }
}
