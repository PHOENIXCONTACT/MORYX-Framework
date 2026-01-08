// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.Logging;
using Moryx.Orders.Assignment;
using Moryx.Orders.Management.Properties;
using Moryx.Tools;

namespace Moryx.Orders.Management.Assignment
{
    [Component(LifeCycle.Singleton, typeof(IOperationAssignStep), Name = nameof(RecipeAssignStep))]
    internal class RecipeAssignStep : IOperationAssignStep, ILoggingComponent
    {
        public IRecipeAssignment RecipeAssignment { get; set; }

        public IProductManagement ProductManagement { get; set; }

        public IOperationDataPool OperationDataPool { get; set; }

        public IJobManagement JobManagement { get; set; }

        public IModuleLogger Logger { get; set; }

        public void Start()
        {
            ProductManagement.TypeChanged += OnTypeChangedAsync;
            ProductManagement.RecipeChanged += OnRecipeChangedAsync;
        }

        public void Stop()
        {
            ProductManagement.TypeChanged -= OnTypeChangedAsync;
            ProductManagement.RecipeChanged -= OnRecipeChangedAsync;
        }

        public async Task<bool> AssignStep(IOperationData operationData, IOperationLogger operationLogger)
        {
            var operation = operationData.Operation;

            // Select recipes
            var selectedRecipes = await RecipeAssignment.SelectRecipesAsync(operation, operationLogger, CancellationToken.None);
            if (!selectedRecipes.Any() || selectedRecipes.Any(r => r == null))
            {
                operationLogger.Log(LogLevel.Error, Strings.RecipeAssignStep_Selection_Failed);
                return false;
            }

            // Process recipes
            var processed = new List<ProductionRecipe>(selectedRecipes.Count);
            foreach (var selected in selectedRecipes)
            {
                // Clone recipe
                var clone = (ProductionRecipe)selected.Clone();
                var successfullyProcessed = await RecipeAssignment.ProcessRecipeAsync(clone, operation, operationLogger, CancellationToken.None);

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
            processed.ForEach(r => ProductManagement.SaveRecipeAsync(r));

            // Add processed to operation
            await operationData.AssignRecipes(processed);

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> RestoreStep(IOperationData operationData, IOperationLogger operationLogger)
        {
            var operation = operationData.Operation;

            // If the operation has already recipes, they will be restored
            if (operation.Recipes.All(r => r is RecipeReference))
            {
                // Existing recipe -> restore
                var recipeTasks = operation.Recipes.Select(reference => ProductManagement.LoadRecipeAsync(reference.Id));
                var restored = (await Task.WhenAll(recipeTasks))
                    .Cast<IProductRecipe>().ToArray();

                // Clear references
                operation.Recipes.Clear();

                // Add restored
                operation.Recipes.AddRange(restored);

                return true;
            }

            operationLogger.Log(LogLevel.Error, Strings.RecipeAssignStep_Restore_Failed);

            return false;
        }

        /// <summary>
        /// Event handler for recipe changes of the product management
        /// </summary>
        private async void OnRecipeChangedAsync(object sender, IRecipe recipe)
        {
            try
            {
                if (recipe is not IProductRecipe productRecipe)
                {
                    return;
                }

                IList<IOperationData> operations;

                // If the recipe is a template itself
                if (productRecipe.TemplateId == 0)
                {
                    operations = [.. OperationDataPool.GetAll(o =>
                        o.State.Classification < OperationStateClassification.Completed &&
                        o.Operation.Recipes.Any(r => r.TemplateId == productRecipe.Id))];
                }
                // Else the recipe is a clone
                else
                {
                    operations = [.. OperationDataPool.GetAll(o =>
                        o.State.Classification < OperationStateClassification.Completed &&
                        o.Operation.Recipes.Any(r => r.Id == productRecipe.Id))];
                }

                // Raise recipe changed for all operations where the recipe was changed
                foreach (var operationData in operations)
                {
                    await operationData.RecipeChanged(productRecipe);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during recipe change handling in {step}", nameof(RecipeAssignStep));
            }
        }

        private async void OnTypeChangedAsync(object sender, ProductType productType)
        {
            try
            {
                // Update not initial operations
                var operations = OperationDataPool.GetAll(o =>
                    o.State.Classification < OperationStateClassification.Completed).ToArray();

                // Try to find affected operations
                await ScanOperationsForChangedType(productType, operations);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during product type change handling in {step}", nameof(RecipeAssignStep));
            }
        }

        private async Task ScanOperationsForChangedType(ProductType productType, IOperationData[] operations)
        {
            foreach (var operationData in operations)
            {
                var recipes = operationData.Operation.Recipes.ToArray();
                await ScanRecipesForChangedType(productType, operationData, recipes);
            }
        }

        private async Task ScanRecipesForChangedType(ProductType productType, IOperationData operationData, IProductRecipe[] recipes)
        {
            foreach (var recipe in recipes)
            {
                // Operation is affected if a recipe referenced the updated product type directly of is a part of a referenced product type
                if (recipe.Product.Id == productType.Id
                    || recipe.Target.Id == productType.Id
                    || CheckProductPartLinksRecursive(recipe.Product, productType.Id))
                {
                    // load a fresh recipe directly from product!
                    var templateRecipe = (IProductRecipe)await ProductManagement.LoadRecipeAsync(recipe.TemplateId);
                    await operationData.RecipeChanged(templateRecipe);
                }
            }
        }

        private static bool CheckProductPartLinksRecursive(ProductType type, long productId)
        {
            var references = ReflectionTool.GetReferences<ProductPartLink>(type)
                .SelectMany(g => g).ToArray();

            if (references.Any(link => link.Product?.Id == productId))
            {
                return true;
            }

            return references.Any(link => CheckProductPartLinksRecursive(link.Product, productId));
        }
    }
}
