// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Threading.Tasks;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Modules;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Will be to assign recipes to the operation based on existing data.
    /// </summary>
    public interface IRecipeAssignment : IConfiguredPlugin<RecipeAssignmentConfig>
    {
        /// <summary>
        /// Returns the possible recipes for the given product identity
        /// </summary>
        Task<IReadOnlyList<IProductRecipe>> PossibleRecipes(ProductIdentity identity);

        /// <summary>
        /// Select a recipe for the current operation
        /// </summary>
        Task<IReadOnlyList<IProductRecipe>> SelectRecipes(Operation operation, IOperationLogger operationLogger);

        /// <summary>
        /// Assigns the recipe to the operation
        /// </summary>
        Task<bool> ProcessRecipe(IProductRecipe clone, Operation operation, IOperationLogger operationLogger);
    }
}