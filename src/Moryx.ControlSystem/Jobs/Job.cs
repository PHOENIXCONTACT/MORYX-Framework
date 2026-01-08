// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Recipes;

namespace Moryx.ControlSystem.Jobs;

/// <summary>
/// External representation of a Job
/// </summary>
public class Job
{
    /// <summary>
    /// Create job
    /// </summary>
    public Job(IRecipe recipe, int amount)
    {
        Recipe = recipe;
        Amount = amount;
    }

    /// <summary>
    /// This Job's ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the recipe.
    /// </summary>
    public IRecipe Recipe { get; protected set; }

    /// <summary>
    /// The number of items to produce
    /// </summary>
    public int Amount { get; protected set; }

    /// <summary>
    /// Number of successful processes
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed processes
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Number of reworked processes
    /// </summary>
    public int ReworkedCount { get; set; }

    /// <summary>
    /// Classification of the job
    /// </summary>
    public JobClassification Classification { get; set; }

    /// <summary>
    /// Detailed display name of the state
    /// TODO: Remove this property in next major and replace with reworked JobClassification
    /// </summary>
    public virtual string StateDisplayName { get; protected set; }

    /// <summary>
    /// Currently running processes of the job
    /// </summary>
    public IReadOnlyList<Process> RunningProcesses
    {
        get => field ?? Array.Empty<Process>();
        protected set;
    }

    /// <summary>
    /// All processes of the job including running and completed processes
    /// </summary>
    public IReadOnlyList<Process> AllProcesses
    {
        get => field ?? Array.Empty<Process>();
        protected set;
    }
}