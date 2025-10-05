// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Recipes;

namespace Moryx.ControlSystem.Setups
{
    /// <summary>
    /// Facade for modules that can determine setup requirement and and create setup recipes
    /// </summary>
    public interface ISetupProvider : IRecipeProvider
    {
        /// <summary>
        /// Determine the necessary changes to produce a given recipe 
        /// </summary>
        /// <returns><value>null</value> if no setup is required, otherwise a setup recipe with a reconfiguration workplan</returns>
        ISetupRecipe RequiredSetup(SetupExecution execution, IProductionRecipe recipe, ISetupTarget targetSystem);
    }
}