// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;

namespace Moryx.AbstractionLayer.Tests.TestData;

public class TestRecipe : Recipe
{
    public override IRecipe Clone() => new TestRecipe();
}
