// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Generic strategy for product instances
    /// </summary>
    [ExpectedConfig(typeof(GenericRecipeConfiguration))]
    [StrategyConfiguration(typeof(IProductRecipe), DerivedTypes = true)]
    [Plugin(LifeCycle.Transient, typeof(IProductRecipeStrategy), Name = nameof(GenericRecipeStrategy))]
    internal class GenericRecipeStrategy : RecipeStrategyBase<GenericRecipeConfiguration>
    {
        /// <summary>
        /// Injected entity mapper
        /// </summary>
        public GenericEntityMapper<ProductionRecipe, IProductType> EntityMapper { get; set; }

        /// <summary>
        /// Initialize the type strategy
        /// </summary>
        public override void Initialize(ProductRecipeConfiguration config)
        {
            base.Initialize(config);

            EntityMapper.Initialize(TargetType, Config);
        }

        public override void SaveRecipe(IProductRecipe source, IGenericColumns target)
        {
            EntityMapper.WriteValue(source, target);
        }

        public override void LoadRecipe(IGenericColumns source, IProductRecipe target)
        {
            EntityMapper.ReadValue(source, target);
        }
    }
}
