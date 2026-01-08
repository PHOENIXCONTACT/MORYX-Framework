// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Management.NullStrategies;

/// <summary>
/// Simple link strategy without any properties
/// </summary>
[PropertylessStrategyConfiguration(typeof(ProductPartLink), DerivedTypes = true)]
[Plugin(LifeCycle.Transient, typeof(IProductLinkStrategy), Name = nameof(SimpleLinkStrategy))]
internal class SimpleLinkStrategy : LinkStrategyBase
{
    public override Task LoadPartLinkAsync(IGenericColumns linkEntity, ProductPartLink target, CancellationToken cancellationToken)
    {
        // We have no custom properties
        return Task.CompletedTask;
    }

    public override Task SavePartLinkAsync(ProductPartLink source, IGenericColumns target, CancellationToken cancellationToken)
    {
        // We have no custom properties
        return Task.CompletedTask;
    }
}