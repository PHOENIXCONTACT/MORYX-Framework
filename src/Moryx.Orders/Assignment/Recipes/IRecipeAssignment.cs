// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Modules;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Will be to assign recipes to the operation based on existing data.
    /// </summary>
    public interface IRecipeAssignment : IAsyncConfiguredPlugin<RecipeAssignmentConfig>
    {
        /// <summary>
        /// Returns the possible recipes for the given product identity
        /// </summary>
        Task<IReadOnlyList<IProductRecipe>> PossibleRecipesAsync(ProductIdentity identity, CancellationToken cancellationToken);

        /// <summary>
        /// Select a recipe for the current operation
        /// </summary>
        Task<IReadOnlyList<IProductRecipe>> SelectRecipesAsync(Operation operation, IOperationLogger operationLogger,
            CancellationToken cancellationToken);

        /// <summary>
        /// Assigns the recipe to the operation
        /// </summary>
        Task<bool> ProcessRecipeAsync(IProductRecipe clone, Operation operation, IOperationLogger operationLogger,
            CancellationToken cancellationToken);
    }
}
