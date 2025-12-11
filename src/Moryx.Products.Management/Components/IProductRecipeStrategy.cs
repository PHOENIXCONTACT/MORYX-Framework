// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Modules;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Interface for plugins that can convert recipes
    /// </summary>
    public interface IProductRecipeStrategy : IConfiguredInitializable<ProductRecipeConfiguration>
    {
        /// <summary>
        /// Target type of this strategy
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Write recipe properties to database generic columns
        /// </summary>
        Task SaveRecipeAsync(IProductRecipe source, IGenericColumns target, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load recipe from database information
        /// </summary>
        Task LoadRecipeAsync(IGenericColumns source, IProductRecipe target, CancellationToken cancellationToken = default);
    }
}
