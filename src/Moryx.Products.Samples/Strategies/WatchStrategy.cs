// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Products.Management;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Samples;

[StrategyConfiguration(typeof(WatchType), DerivedTypes = false)]
[Plugin(LifeCycle.Transient, typeof(IProductTypeStrategy), Name = nameof(WatchStrategy))]
public class WatchStrategy : TypeStrategyBase
{
    /// <inheritdoc />
    public override bool HasChanged(ProductType current, IGenericColumns dbProperties)
    {
        var watch = (WatchType)current;
        return Math.Abs(watch.Weight - dbProperties.Float1) > 0.01
               || Math.Abs(watch.Price - dbProperties.Float2) > 0.01;
    }

    /// <inheritdoc />
    public override Task SaveTypeAsync(ProductType source, IGenericColumns target, CancellationToken cancellationToken)
    {
        var watch = (WatchType)source;
        target.Float1 = watch.Weight;
        target.Float2 = watch.Price;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task LoadTypeAsync(IGenericColumns source, ProductType target, CancellationToken cancellationToken)
    {
        var watch = (WatchType)target;
        watch.Weight = source.Float1;
        watch.Price = source.Float2;

        return Task.CompletedTask;
    }

    public override Expression<Func<IGenericColumns, bool>> TransformSelector<TProduct>(Expression<Func<TProduct, bool>> selector)
    {
        throw new NotImplementedException();
    }
}

[StrategyConfiguration(typeof(WatchInstance), DerivedTypes = false)]
[Plugin(LifeCycle.Transient, typeof(IProductInstanceStrategy), Name = nameof(WatchInstanceStrategy))]
public class WatchInstanceStrategy : InstanceStrategyBase
{
    public override Expression<Func<IGenericColumns, bool>> TransformSelector<TInstance>(Expression<Func<TInstance, bool>> selector)
    {
        return c => c.Integer2 == 1;
    }

    /// <inheritdoc />
    public override Task SaveInstanceAsync(ProductInstance source, IGenericColumns target, CancellationToken cancellationToken)
    {
        var watch = (WatchInstance)source;
        target.Integer1 = watch.TimeSet ? 1 : 0;
        target.Integer2 = watch.DeliveryDate.ToBinary();

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task LoadInstanceAsync(IGenericColumns source, ProductInstance target, CancellationToken cancellationToken)
    {
        var watch = (WatchInstance)target;
        watch.TimeSet = source.Integer1 == 1;
        watch.DeliveryDate = DateTime.FromBinary(source.Integer2);

        return Task.CompletedTask;
    }
}

[StrategyConfiguration(typeof(NeedlePartLink))]
[Plugin(LifeCycle.Transient, typeof(IProductLinkStrategy), Name = nameof(NeedleLinkStrategy))]
public class NeedleLinkStrategy : LinkStrategyBase
{
    public override Task LoadPartLinkAsync(IGenericColumns linkEntity, ProductPartLink target, CancellationToken cancellationToken)
    {
        var link = (NeedlePartLink)target;
        link.Role = (NeedleRole)linkEntity.Integer1;

        return Task.CompletedTask;
    }

    public override Task SavePartLinkAsync(ProductPartLink source, IGenericColumns target, CancellationToken cancellationToken)
    {
        var link = (NeedlePartLink)source;
        target.Integer1 = (int)link.Role;

        return Task.CompletedTask;
    }
}