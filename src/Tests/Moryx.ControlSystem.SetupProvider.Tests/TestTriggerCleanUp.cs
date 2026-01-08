// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.SetupProvider.Tests
{
    internal class TestTriggerCleanup : TestTriggerBase
    {
        public override SetupExecution Execution => SetupExecution.AfterProduction;

        public override SetupEvaluation Evaluate(IProductRecipe recipe)
        {
            RequiredWasCalled = true;

            return SetupEvaluation.Remove(new TestSetupCapabilities
            {
                SetupState = ((TestRecipe)recipe).SetupState
            });
        }
    }
}

