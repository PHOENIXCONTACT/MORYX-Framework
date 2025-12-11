// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.Workplans;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Setup
{
    [Component(LifeCycle.Singleton, Name = nameof(TemporarySetupProvider))]
    internal class TemporarySetupProvider : IRecipeProvider
    {
        public string Name => nameof(TemporarySetupProvider);

        public Task<IRecipe> LoadRecipeAsync(long id)
        {
            var setup = new SetupRecipe
            {
                Id = id,
                Origin = this,
                Name = "After Production",
                Execution = SetupExecution.AfterProduction,
                Workplan = new Workplan(),
                Classification = RecipeClassification.Default
            };
            return Task.FromResult<IRecipe>(setup);
        }

        public SetupRecipe CreateTemporary(ProductionRecipe recipe)
        {
            var setup = new SetupRecipe
            {
                Origin = this,
                Name = $"After {recipe.Workplan.Name}",
                Execution = SetupExecution.AfterProduction,
                Workplan = new Workplan(),
                TargetRecipe = recipe,
                Classification = RecipeClassification.Default
            };
            return setup;
        }

        public event EventHandler<IRecipe> RecipeChanged;
    }
}
