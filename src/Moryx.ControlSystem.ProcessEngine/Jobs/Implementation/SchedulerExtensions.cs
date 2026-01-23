// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

internal static class SchedulerExtensions
{
    /// <param name="one"></param>
    extension(Job one)
    {
        /// <summary>
        /// Check if two jobs have the same recipe
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool SameRecipeAs(Job other)
        {
            if (one == null || other == null)
                return false;

            if (one.Recipe is SetupRecipe || other.Recipe is SetupRecipe)
                return false; // Setups can not be identical

            return one.Recipe.Id == other.Recipe.Id;
        }

        /// <summary>
        /// Check if the job is a preparation job
        /// </summary>
        public bool IsPrepare()
        {
            return one != null
                   && one.Recipe is SetupRecipe prepare
                   && prepare.Execution == SetupExecution.BeforeProduction;
        }

        /// <summary>
        /// Check if the job is a preparation job for a specific job
        /// </summary>
        public bool IsPrepareOf(Job target)
        {
            return IsPrepare(one) && ((SetupRecipe)one.Recipe).TargetRecipe.Id == target?.Recipe.Id;
        }

        /// <summary>
        /// Check if the job is a clean-up job
        /// </summary>
        public bool IsCleanup()
        {
            return one != null
                   && one.Recipe is SetupRecipe cleanup
                   && cleanup.Execution == SetupExecution.AfterProduction;
        }

        /// <summary>
        /// Check if the job is a clean-up job for a specific job
        /// </summary>
        public bool IsCleanupOf(Job target)
        {
            return IsCleanup(one) && ((SetupRecipe)one.Recipe).TargetRecipe.Id == target?.Recipe.Id;
        }
    }
}
