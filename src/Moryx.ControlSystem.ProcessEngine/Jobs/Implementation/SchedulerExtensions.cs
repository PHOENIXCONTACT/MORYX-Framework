// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

internal static class SchedulerExtensions
{
    /// <summary>
    /// Check if two jobs have the same recipe
    /// </summary>
    /// <param name="one"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool SameRecipeAs(this Job one, Job other)
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
    public static bool IsPrepare(this Job candidate)
    {
        return candidate != null
               && candidate.Recipe is SetupRecipe prepare
               && prepare.Execution == SetupExecution.BeforeProduction;
    }

    /// <summary>
    /// Check if the job is a preparation job for a specific job
    /// </summary>
    public static bool IsPrepareOf(this Job candidate, Job target)
    {
        return IsPrepare(candidate) && ((SetupRecipe)candidate.Recipe).TargetRecipe == target?.Recipe;
    }

    /// <summary>
    /// Check if the job is a clean-up job
    /// </summary>
    public static bool IsCleanup(this Job candidate)
    {
        return candidate != null
               && candidate.Recipe is SetupRecipe cleanup
               && cleanup.Execution == SetupExecution.AfterProduction;
    }

    /// <summary>
    /// Check if the job is a clean-up job for a specific job
    /// </summary>
    public static bool IsCleanupOf(this Job candidate, Job target)
    {
        return IsCleanup(candidate) && ((SetupRecipe)candidate.Recipe).TargetRecipe == target?.Recipe;
    }
}