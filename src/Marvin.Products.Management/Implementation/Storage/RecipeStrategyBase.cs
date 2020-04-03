// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Recipes;
using Marvin.Products.Model;
using Marvin.Tools;

namespace Marvin.Products.Management
{
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
        public override void Initialize(ProductRecipeConfiguration config)
        {
            base.Initialize(config);


            TargetType = ReflectionTool.GetPublicClasses<IProductRecipe>(p => p.Name == config.TargetType).FirstOrDefault();
        }

        /// <inheritdoc />
        public abstract void SaveRecipe(IProductRecipe source, IGenericColumns target);

        /// <inheritdoc />
        public abstract void LoadRecipe(IGenericColumns source, IProductRecipe target);
    }
}
