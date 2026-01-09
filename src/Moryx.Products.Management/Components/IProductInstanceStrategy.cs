// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Products;
using Moryx.Modules;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Management;

/// <summary>
/// Strategy for loading and saving product instances
/// </summary>
public interface IProductInstanceStrategy : IAsyncConfiguredInitializable<ProductInstanceConfiguration>
{
    /// <summary>
    /// Target type of this strategy
    /// </summary>
    Type TargetType { get; }

    /// <summary>
    /// Flag if instances of this product type shall be skipped when loading or saving instances
    /// </summary>
    bool SkipInstances { get; }

    /// <summary>
    /// Transform a product class selector to a database compatible expression
    /// </summary>
    Expression<Func<IGenericColumns, bool>> TransformSelector<TInstance>(Expression<Func<TInstance, bool>> selector);

    /// <summary>
    /// Save instance to database
    /// </summary>
    Task SaveInstanceAsync(ProductInstance source, IGenericColumns target, CancellationToken cancellationToken);

    /// <summary>
    /// Load additional instance properties from entity and write them to the business object
    /// </summary>
    Task LoadInstanceAsync(IGenericColumns source, ProductInstance target, CancellationToken cancellationToken);
}