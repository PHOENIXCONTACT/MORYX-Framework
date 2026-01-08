// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.Modules;

namespace Moryx.ControlSystem.SetupProvider
{
    internal interface ISetupManager : IPlugin
    {
        SetupRecipe RequiredSetup(SetupExecution execution, ProductionRecipe recipe, ISetupTarget targetSystem);
    }
}
