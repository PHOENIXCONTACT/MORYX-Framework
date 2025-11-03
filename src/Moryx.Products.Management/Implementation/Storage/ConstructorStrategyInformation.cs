// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Modules;
using System.Reflection;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Management.Implementation.Storage
{
    internal class ConstructorStrategyInformation<TObject, TConfiguration, TStrategy> where TConfiguration : IPluginConfig where TStrategy : IConfiguredPlugin<TConfiguration>
    {
        public string Identifier { get; set; }
        public Func<TObject> Constructor { get; set; }
        public TStrategy Strategy { get; set; }
    }

    internal class ProductTypeInformation
    {
        public string Identifier { get; set; }
        public Func<ProductType> Constructor { get; set; }

        public Type Type { get; private set; }

        public IProductTypeStrategy Strategy { get; set; }

        public IDictionary<string, ConstructorStrategyInformation<IProductPartLink, ProductLinkConfiguration, IProductLinkStrategy>> PartLinksInformation =
            new Dictionary<string, ConstructorStrategyInformation<IProductPartLink, ProductLinkConfiguration, IProductLinkStrategy>>();

        private IDictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>();

        public ProductTypeInformation(Type type)
        {
            Type = type;
            foreach (var propInfo in type.GetProperties())
            {
                _properties.Add(propInfo.Name, propInfo);
            }
        }

        public ProductType CreateTypeFromEntity(ProductTypeEntity entity)
        {
            var productType = Constructor();
            productType.Name = entity.Name;
            productType.Id = entity.Id;
            productType.Identity = new ProductIdentity(entity.Identifier, entity.Revision);

            ProductInstance v;

            return productType;
        }

        public IList<PartLinkInfo> GetAllPartLinks(IProductType modifiedInstance)
        {
            var result = new List<PartLinkInfo>();
            foreach (var linkStrategy in PartLinksInformation.Values.Select(p => p.Strategy))
            {
                var property = _properties[linkStrategy.PropertyName];
                var value = property.GetValue(modifiedInstance);
                var partLink = new PartLinkInfo(linkStrategy, value);
                if (typeof(IProductPartLink).IsAssignableFrom(property.PropertyType))
                    partLink.Type = PartLinkType.single;
                else if (typeof(IEnumerable<IProductPartLink>).IsAssignableFrom(property.PropertyType))
                    partLink.Type = PartLinkType.list;
                result.Add(partLink);
            }
            return result;
        }

        public ProductTypeWrapper GetTypeWrapper()
        {
            var partLinks = new Dictionary<string, Func<IProductPartLink>>();
            foreach (var partLinkInfo in PartLinksInformation)
                partLinks.Add(partLinkInfo.Key, partLinkInfo.Value.Constructor);
            return new ProductTypeWrapper(Identifier, Constructor, partLinks, _properties.Values.ToList());
        }

    }

    internal class PartLinkInfo
    {
        public IProductLinkStrategy ProductLinkStrategy;
        public object Value;
        public PartLinkType Type;

        public PartLinkInfo(IProductLinkStrategy productLinkStrategy, object value)
        {
            ProductLinkStrategy = productLinkStrategy;
            Value = value;
            Type = PartLinkType.none;
        }

    }

    internal enum PartLinkType
    {
        none,
        single,
        list
    }

}

