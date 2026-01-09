// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Products.Management.Model;
using Moryx.Tools;

namespace Moryx.Products.Management;

/// <summary>
/// Non-generic base class for <see cref="IProductInstanceStrategy"/>
/// </summary>
public abstract class RecipeStrategyBase : RecipeStrategyBase<ProductRecipeConfiguration>
{
}

/// <summary>
/// Base class for all <see cref="IProductInstanceStrategy"/>
/// </summary>
/// <typeparam name="TConfig"></typeparam>
public abstract class RecipeStrategyBase<TConfig> : StrategyBase<TConfig, ProductRecipeConfiguration>, IProductRecipeStrategy
    where TConfig : ProductRecipeConfiguration
{
    /// <inheritdoc />
    public override async Task InitializeAsync(ProductRecipeConfiguration config, CancellationToken cancellationToken = default)
    {
        await base.InitializeAsync(config, cancellationToken);

        TargetType = ReflectionTool.GetPublicClasses<IProductRecipe>(p => p.FullName == config.TargetType).FirstOrDefault();
    }

    /// <inheritdoc />
    public abstract Task SaveRecipeAsync(IProductRecipe source, IGenericColumns target, CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task LoadRecipeAsync(IGenericColumns source, IProductRecipe target, CancellationToken cancellationToken);
}