// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Products.Model;

namespace Moryx.Products.Management.NullStrategies
{
    /// <summary>
    /// Simple link strategy without any properties
    /// </summary>
    [PropertylessStrategyConfiguration(typeof(IProductPartLink), DerivedTypes = true)]
    [Plugin(LifeCycle.Transient, typeof(IProductLinkStrategy), Name = nameof(SimpleLinkStrategy))]
    internal class SimpleLinkStrategy : LinkStrategyBase
    {
        public override void LoadPartLink(IGenericColumns linkEntity, IProductPartLink target)
        {
            // We have no custom properties
        }

        public override void SavePartLink(IProductPartLink source, IGenericColumns target)
        {
            // We have no custom properties
        }
    }
}
