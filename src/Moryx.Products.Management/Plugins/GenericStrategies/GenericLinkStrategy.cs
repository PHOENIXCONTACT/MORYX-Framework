// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Products.Model;

namespace Moryx.Products.Management
{
    /// <summary>
    /// 
    /// </summary>
    [ExpectedConfig(typeof(GenericLinkConfiguration))]
    [StrategyConfiguration(typeof(IProductPartLink), DerivedTypes = true)]
    [Plugin(LifeCycle.Transient, typeof(IProductLinkStrategy), Name = nameof(GenericLinkStrategy))]
    internal class GenericLinkStrategy : LinkStrategyBase<GenericLinkConfiguration>
    {
        /// <summary>
        /// Injected entity mapper
        /// </summary>
        public GenericEntityMapper<ProductPartLink, ProductType> EntityMapper { get; set; }

        /// <summary>
        /// Initialize the type strategy
        /// </summary>
        public override void Initialize(ProductLinkConfiguration config)
        {
            base.Initialize(config);

            var property = TargetType.GetProperty(PropertyName);
            var linkType = property.PropertyType;
            // Extract element type from collections
            if (typeof(IEnumerable<IProductPartLink>).IsAssignableFrom(linkType))
            {
                linkType = linkType.GetGenericArguments()[0];
            }

            EntityMapper.Initialize(linkType, Config);
        }

        public override void LoadPartLink(IGenericColumns linkEntity, IProductPartLink target)
        {
            EntityMapper.ReadValue(linkEntity, target);
        }

        public override void SavePartLink(IProductPartLink source, IGenericColumns target)
        {
            EntityMapper.WriteValue(source, target);
        }
    }
}
