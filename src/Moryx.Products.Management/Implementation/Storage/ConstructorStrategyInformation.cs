// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.AbstractionLayer.Products;
using Moryx.Modules;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Management
{
    internal class ConstructorStrategyInformation<TObject, TConfiguration, TStrategy> where TConfiguration : IPluginConfig where TStrategy : IAsyncConfiguredInitializable<TConfiguration>
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

        public IDictionary<string, ConstructorStrategyInformation<ProductPartLink, ProductLinkConfiguration, IProductLinkStrategy>> PartLinksInformation =
            new Dictionary<string, ConstructorStrategyInformation<ProductPartLink, ProductLinkConfiguration, IProductLinkStrategy>>();

        private IDictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>();

        public ProductTypeInformation(Type type)
        {
            Type = type;
            foreach (var propInfo in type.GetProperties())
            {
                if (_properties.TryAdd(propInfo.Name, propInfo))
                {
                    continue;
                }
                else if (propInfo.DeclaringType == type)
                {
                    _properties[propInfo.Name] = propInfo;
                }
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

        public IList<PartLinkInfo> GetAllPartLinks(ProductType modifiedInstance)
        {
            var result = new List<PartLinkInfo>();
            foreach (var linkStrategy in PartLinksInformation.Values.Select(p => p.Strategy))
            {
                var property = _properties[linkStrategy.PropertyName];
                var value = property.GetValue(modifiedInstance);
                var partLink = new PartLinkInfo(linkStrategy, value);
                if (typeof(ProductPartLink).IsAssignableFrom(property.PropertyType))
                    partLink.Type = PartLinkType.single;
                else if (typeof(IEnumerable<ProductPartLink>).IsAssignableFrom(property.PropertyType))
                    partLink.Type = PartLinkType.list;
                result.Add(partLink);
            }
            return result;
        }

        public ProductTypeWrapper GetTypeWrapper()
        {
            var partLinks = new Dictionary<string, Func<ProductPartLink>>();
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

