// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Modules;
using Moryx.Products.Model;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Interface for plugins that can convert recipes
    /// </summary>
    public interface IProductRecipeStrategy : IConfiguredPlugin<ProductRecipeConfiguration>
    {
        /// <summary>
        /// Target type of this strategy
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Write recipe properties to database generic columns
        /// </summary>
        void SaveRecipe(IProductRecipe source, IGenericColumns target);

        /// <summary>
        /// Load recipe from database information
        /// </summary>
        void LoadRecipe(IGenericColumns source, IProductRecipe target);
    }
}
