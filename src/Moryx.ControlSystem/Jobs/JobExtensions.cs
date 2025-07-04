﻿// Copyright (c) 2024, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.Jobs
{
    /// <summary>
    /// Extensions methods for <see cref="Job"/>s.
    /// </summary>
    public static class JobExtensions
    {
        /// <summary>
        /// Checks whether the <paramref name="job"/> is an <see cref="IPredictiveJob"/>
        /// and gets the predicted failure count; otherwise returns 0.
        /// </summary>
        public static int CountPredictedFailures(this Job job)
        {
            if (job is IPredictiveJob predictiveJob)
                return predictiveJob.PredictedFailures.Count;

            return 0;
        }

        /// <summary>
        /// Checks if the <paramref name="job"/> references an <see cref="IProductionRecipe"/>
        /// </summary>
        public static bool IsProduction(this Job job) => job.Recipe is IProductionRecipe;

        /// <summary>
        /// Checks if the <paramref name="job"/> references an <see cref="ISetupRecipe"/>
        /// </summary>
        public static bool IsSetup(this Job job) => job.Recipe is ISetupRecipe;

        /// <summary>
        /// Checks if the <paramref name="job"/> holds an <see cref="ISetupRecipe"/> 
        /// that is set to be executed <see cref="SetupExecution.BeforeProduction"/>
        /// </summary>
        public static bool IsPreparingSetup(this Job job) 
            => job.Recipe is ISetupRecipe setup && setup.Execution == SetupExecution.BeforeProduction;

        /// <summary>
        /// Checks if the <paramref name="job"/> holds an <see cref="ISetupRecipe"/> 
        /// that is set to be executed <see cref="SetupExecution.AfterProduction"/>
        /// </summary>
        public static bool IsCleaningUpSetup(this Job job) 
            => job.Recipe is ISetupRecipe setup && setup.Execution == SetupExecution.AfterProduction;
    }
}
