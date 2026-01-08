// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.Workplans;
using System.Collections.Generic;

namespace Moryx.ControlSystem.SetupProvider.Tests
{
    internal class MultiStepTrigger : TestTriggerBase
    {
        public override SetupExecution Execution => SetupExecution.BeforeProduction;

        public override SetupEvaluation Evaluate(IProductRecipe recipe)
        {
            RequiredWasCalled = true;

            return SetupEvaluation.Provide(new TestSetupCapabilities
            {
                SetupState = ((TestRecipe)recipe).SetupState
            });
        }

        public override IReadOnlyList<IWorkplanStep> CreateSteps(IProductRecipe recipe)
        {
            CreateStepCalled = true;
            return [new TestSetupTask { Name = "P1" }, new TestSetupTask { Name = "P2" }];
        }
    }
}

