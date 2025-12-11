// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;

namespace Moryx.ControlSystem.ProcessEngine.Tests
{
    internal class TestRecipe : ProductionRecipe
    {
        public int SetupState { get; set; }
    }
}

