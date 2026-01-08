// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Products;
using Moryx.Products.Management.Model;
using Moryx.Tools;

namespace Moryx.Products.Management;

/// <summary>
/// Base class for product strategies
/// </summary>
public abstract class TypeStrategyBase : TypeStrategyBase<ProductTypeConfiguration>
{
}

/// <summary>
/// Base class for product strategies
/// </summary>
public abstract class TypeStrategyBase<TConfig> : StrategyBase<TConfig, ProductTypeConfiguration>, IProductTypeStrategy
    where TConfig : ProductTypeConfiguration
{
    /// <inheritdoc />
    public override async Task InitializeAsync(ProductTypeConfiguration config, CancellationToken cancellationToken = default)
    {
        await base.InitializeAsync(config, cancellationToken);

        TargetType = ReflectionTool.GetPublicClasses<ProductType>(p => p.FullName == config.TargetType).FirstOrDefault();
    }

    /// <inheritdoc />
    public abstract bool HasChanged(ProductType current, IGenericColumns dbProperties);

    /// <inheritdoc />
    public abstract Task LoadTypeAsync(IGenericColumns source, ProductType target, CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task SaveTypeAsync(ProductType source, IGenericColumns target, CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Expression<Func<IGenericColumns, bool>> TransformSelector<TProduct>(Expression<Func<TProduct, bool>> selector);
}