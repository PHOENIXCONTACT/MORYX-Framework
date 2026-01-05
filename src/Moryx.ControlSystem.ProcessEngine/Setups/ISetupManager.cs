// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.Modules;

namespace Moryx.ControlSystem.ProcessEngine.Setups
{
    /// <summary>
    /// Component that can determine setup requirement and create setup recipes
    /// </summary>
    internal interface ISetupManager : IPlugin, IRecipeProvider
    {
        SetupRecipe RequiredSetup(SetupExecution execution, ProductionRecipe recipe, ISetupTarget targetSystem);
    }
}
