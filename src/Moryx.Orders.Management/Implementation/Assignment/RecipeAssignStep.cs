// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.Orders.Assignment;
using Moryx.Orders.Management.Properties;
using Moryx.Tools;

namespace Moryx.Orders.Management.Assignment
{
    [Component(LifeCycle.Singleton, typeof(IOperationAssignStep), Name = nameof(RecipeAssignStep))]
    internal class RecipeAssignStep : IOperationAssignStep
    {
        public IRecipeAssignment RecipeAssignment { get; set; }

        public IProductManagement ProductManagement { get; set; }

        public IOperationDataPool OperationDataPool { get; set; }

        public IJobManagement JobManagement { get; set; }

        public void Start()
        {
            ProductManagement.TypeChanged += OnTypeChanged;
            ProductManagement.RecipeChanged += OnRecipeChanged;
        }

        public void Stop()
        {
            ProductManagement.TypeChanged += OnTypeChanged;
            ProductManagement.RecipeChanged += OnRecipeChanged;
        }

        public async Task<bool> AssignStep(IOperationData operationData, IOperationLogger operationLogger)
        {
            var operation = operationData.Operation;

            // Select recipes
            var selectedRecipes = await RecipeAssignment.SelectRecipes(operation, operationLogger);
            if (!selectedRecipes.Any() || selectedRecipes.Any(r => r == null))
            {
                operationLogger.Log(LogLevel.Error, Strings.RecipeAssignStep_Selection_Failed);
                return false;
            }

            // Process recipes
            var processed = new List<IProductionRecipe>(selectedRecipes.Count);
            foreach (var selected in selectedRecipes)
            {
                // Clone recipe
                var clone = (IProductionRecipe)selected.Clone();
                var successfullyProcessed = await RecipeAssignment.ProcessRecipe(clone, operation, operationLogger);

                if (!successfullyProcessed)
                {
                    operationLogger.Log(LogLevel.Error, Strings.RecipeAssignStep_Processing_Failed, clone.Name);
                    return false;
                }

                processed.Add(clone);
            }

            // Evaluate recipe workplans on job management
            foreach (var recipe in processed)
            {
                var evaluation = JobManagement.Evaluate(recipe, operation.TargetAmount);
                if (evaluation.WorkplanErrors.Count > 0)
                {
                    var workplanError = string.Join("\n", evaluation.WorkplanErrors.Distinct());
                    operationLogger.Log(LogLevel.Error, Strings.RecipeAssignStep_WorkplanValidation_Failed, workplanError);
                    return false;
                }
            }

            // Save recipes
            processed.ForEach(r => ProductManagement.SaveRecipe(r));

            // Add processed to operation
            operationData.AssignRecipes(processed);

            return true;
        }

        /// <inheritdoc />
        public Task<bool> RestoreStep(IOperationData operationData, IOperationLogger operationLogger)
        {
            var operation = operationData.Operation;

            // If the operation has already recipes, they will be restored
            if (operation.Recipes.All(r => r is RecipeReference))
            {
                // Existing recipe -> restore
                var restored = operation.Recipes.Select(reference => (IProductRecipe)ProductManagement.LoadRecipe(reference.Id))
                    .ToArray();

                // Clear references
                operation.Recipes.Clear();

                // Add restored
                operation.Recipes.AddRange(restored);

                return Task.FromResult(true);
            }

            operationLogger.Log(LogLevel.Error, Strings.RecipeAssignStep_Restore_Failed);

            return Task.FromResult(false);
        }

        /// <summary>
        /// Event handler for recipe changes of the product management
        /// </summary>
        private void OnRecipeChanged(object sender, IRecipe recipe)
        {
            if (recipe is not IProductRecipe productRecipe)
                return;

            IList<IOperationData> operations;

            // If the recipe is a template itself
            if (productRecipe.TemplateId == 0)
            {
                operations = OperationDataPool.GetAll(o =>
                    o.State.Classification < OperationStateClassification.Completed &&
                    o.Operation.Recipes.Any(r => r.TemplateId == productRecipe.Id)).ToList();
            }
            // Else the recipe is a clone
            else
            {
                operations = OperationDataPool.GetAll(o =>
                    o.State.Classification < OperationStateClassification.Completed &&
                    o.Operation.Recipes.Any(r => r.Id == productRecipe.Id)).ToList();
            }

            // Raise recipe changed for all operations where the recipe was changed
            foreach (var operationData in operations)
                operationData.RecipeChanged(productRecipe);
        }

        private void OnTypeChanged(object sender, ProductType productType)
        {
            // Update not initial operations
            var operations = OperationDataPool.GetAll(o =>
                o.State.Classification < OperationStateClassification.Completed).ToArray();

            // Try to find affected operations
            foreach (var operationData in operations)
            {
                var recipes = operationData.Operation.Recipes.ToArray();

                foreach (var recipe in recipes)
                {
                    var references = ReflectionTool.GetReferences<IProductPartLink>(recipe.Product)
                        .SelectMany(g => g).ToList();

                    // Operation is affected if a recipe referenced the updated product type directly of is a part of a referenced product type
                    if (recipe.Product.Id == productType.Id || references.Any(r => r.Product.Id == productType.Id))
                    {
                        operationData.RecipeChanged(recipe);
                    }
                }
            }
        }
    }
}
