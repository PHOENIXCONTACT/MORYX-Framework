// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.Jobs;

/// <summary>
/// Extensions methods for <see cref="Job"/>s.
/// </summary>
public static class JobExtensions
{
    extension(Job job)
    {
        /// <summary>
        /// Checks whether the <paramref name="job"/> is an <see cref="IPredictiveJob"/>
        /// and gets the predicted failure count; otherwise returns 0.
        /// </summary>
        public int CountPredictedFailures()
        {
            if (job is IPredictiveJob predictiveJob)
                return predictiveJob.PredictedFailures.Count;

            return 0;
        }

        /// <summary>
        /// Checks if the <paramref name="job"/> references an <see cref="ProductionRecipe"/>
        /// </summary>
        public bool IsProduction() => job.Recipe is ProductionRecipe;

        /// <summary>
        /// Checks if the <paramref name="job"/> references an <see cref="SetupRecipe"/>
        /// </summary>
        public bool IsSetup() => job.Recipe is SetupRecipe;

        /// <summary>
        /// Checks if the <paramref name="job"/> holds an <see cref="SetupRecipe"/> 
        /// that is set to be executed <see cref="SetupExecution.BeforeProduction"/>
        /// </summary>
        public bool IsPreparingSetup()
            => job.Recipe is SetupRecipe setup && setup.Execution == SetupExecution.BeforeProduction;

        /// <summary>
        /// Checks if the <paramref name="job"/> holds an <see cref="SetupRecipe"/> 
        /// that is set to be executed <see cref="SetupExecution.AfterProduction"/>
        /// </summary>
        public bool IsCleaningUpSetup()
            => job.Recipe is SetupRecipe setup && setup.Execution == SetupExecution.AfterProduction;
    }
}