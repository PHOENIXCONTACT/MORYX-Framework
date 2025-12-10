// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Logging;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Base class for assigning a product. Implements <see cref="IRecipeAssignment"/>
    /// </summary>
    /// <typeparam name="TConfig">Special config type of the implementation</typeparam>
    public abstract class RecipeAssignmentBase<TConfig> : IRecipeAssignment
        where TConfig : RecipeAssignmentConfig
    {
        /// <summary>
        /// ProductManagement used to provide the current recipe of a product.
        /// The recipe will be assigned to the operation
        /// </summary>
        public IProductManagement ProductManagement { get; set; }

        /// <summary>
        /// Logger for the specific implementation of <see cref="IProductAssignment"/>
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Typed config for this component
        /// </summary>
        protected TConfig Config { get; private set; }

        /// <inheritdoc cref="IProductAssignment"/>
        public virtual Task InitializeAsync(RecipeAssignmentConfig config)
        {
            Config = (TConfig)config;
            Logger = Logger.GetChild(Config.PluginName, GetType());
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task StartAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task StopAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task<IReadOnlyList<IProductRecipe>> PossibleRecipes(ProductIdentity identity)
        {
            var product = ProductManagement.LoadType(identity);
            if (product == null)
                return Task.FromResult<IReadOnlyList<IProductRecipe>>(Array.Empty<IProductRecipe>());

            var recipes = ProductManagement.GetRecipes(product, RecipeClassification.Default | RecipeClassification.Alternative);
            return Task.FromResult(recipes);
        }

        /// <summary>
        /// Select a recipe for the current operation
        /// </summary>
        public abstract Task<IReadOnlyList<IProductRecipe>> SelectRecipes(Operation operation, IOperationLogger operationLogger);

        /// <summary>
        /// Assigns the recipe to the operation
        /// </summary>
        public abstract Task<bool> ProcessRecipe(IProductRecipe clone, Operation operation, IOperationLogger operationLogger);

        /// <summary>
        /// Default implementation to assign the current recipe to the operation
        /// Will use the given product
        /// </summary>
        protected Task<IProductRecipe> LoadDefaultRecipe(ProductType sourceProduct)
        {
            var defaultRecipe = ProductManagement.GetRecipes(sourceProduct, RecipeClassification.Default)
                .SingleOrDefault();

            return Task.FromResult(defaultRecipe);
        }
    }
}
