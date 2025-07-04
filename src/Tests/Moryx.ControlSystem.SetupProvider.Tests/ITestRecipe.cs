// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;

namespace Moryx.ControlSystem.SetupProvider.Tests
{
    internal interface ITestRecipe : IProductRecipe
    {
        int SetupState { get; set; }
    }
}

