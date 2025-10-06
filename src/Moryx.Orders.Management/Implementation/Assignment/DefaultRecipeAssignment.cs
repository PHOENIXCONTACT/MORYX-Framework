// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.Recipes;
using Moryx.Orders.Assignment;

namespace Moryx.Orders.Management.Assignment
{
    /// <summary>
    /// Default recipe assignment. Loads the recipe with classification <see cref="RecipeClassification.Default"/> and fills properties
    /// </summary>
    [Plugin(LifeCycle.Singleton, typeof(IRecipeAssignment), Name = nameof(DefaultRecipeAssignment))]
    public class DefaultRecipeAssignment : RecipeAssignmentBase<RecipeAssignmentConfig>
    {
        /// <inheritdoc />
        public override async Task<IReadOnlyList<IProductRecipe>> SelectRecipes(Operation operation, IOperationLogger operationLogger)
        {
            if (operation.CreationContext != null &&
                operation.CreationContext.RecipePreselection != 0)
            {
                var recipe = ProductManagement.LoadRecipe(operation.CreationContext.RecipePreselection);
                return new[] { (IProductRecipe)recipe };
            }

            if (operation.Recipes.Any() && operation.Recipes.First() is IRecipe template && template.TemplateId != 0)
            {
                var recipe = ProductManagement.LoadRecipe(template.TemplateId);
                return new[] { (IProductRecipe)recipe };
            }

            return new[] { await LoadDefaultRecipe(operation.Product) };
        }

        /// <inheritdoc />
        public override Task<bool> ProcessRecipe(IProductRecipe clone, Operation operation, IOperationLogger operationLogger)
        {
            // Copy values from known recipe types

            // IOrderBasedRecipe
            if (clone is IOrderBasedRecipe orderBasedRecipe)
            {
                orderBasedRecipe.OrderNumber = operation.Order.Number;
                orderBasedRecipe.OperationNumber = operation.Number;
            }

            return Task.FromResult(true);
        }
    }
}
