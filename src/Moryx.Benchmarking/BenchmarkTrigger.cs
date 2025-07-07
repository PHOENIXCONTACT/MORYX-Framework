﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.Setups;
using Moryx.Modules;
using Moryx.Workplans;
using System.Collections.Generic;

namespace Moryx.Benchmarking
{
    public class BenchmarkTriggerConfig : SetupTriggerConfig
    {
        public override string PluginName => nameof(BenchmarkTrigger);
    }

    [ExpectedConfig(typeof(BenchmarkTriggerConfig))]
    [Plugin(LifeCycle.Transient, typeof(ISetupTrigger), Name = nameof(BenchmarkTrigger))]
    public class BenchmarkTrigger : SetupTriggerBase<SetupTriggerConfig>
    {
        public override SetupExecution Execution => SetupExecution.BeforeProduction;

        public override SetupEvaluation Evaluate(IProductRecipe recipe)
        {
            return SetupClassification.Manual;
        }

        public override IReadOnlyList<IWorkplanStep> CreateSteps(IProductRecipe recipe)
        {
            return new[]
            {
                new BenchmarkStep
                {
                    Parameters = new BenchmarkParameters
                    {
                        Step = 42
                    }
                }
            };
        }
    }
}
