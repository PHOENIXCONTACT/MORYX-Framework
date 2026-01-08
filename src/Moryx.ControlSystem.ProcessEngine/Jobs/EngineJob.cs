// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.Tools;
using Process = Moryx.AbstractionLayer.Processes.Process;

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

/// <summary>
/// Job that includes the prediction feature
/// </summary>
[DebuggerDisplay(nameof(EngineJob) + " <Id: {" + nameof(Id) + "}, Recipe: {" + nameof(Recipe) + "}, Classification: {" + nameof(Classification) + "}>")]
internal class EngineJob : Job, IPredictiveJob
{
    public List<Process> Running { get; set; }

    public List<Process> PredictedFailures { get; set; }

    public List<Process> TotalProcesses { get; set; }

    public EngineJob(IRecipe recipe, int amount) : base(recipe, amount)
    {
        // Prepare both process lists
        PredictedFailures = new ProcessList<Process>();
        RunningProcesses = Running = new ProcessList<Process>(32);
        AllProcesses = TotalProcesses = new ProcessList<Process>(amount);
    }

    public void UpdateRecipe(IRecipe recipe) => Recipe = recipe;

    public void UpdateStateDisplayName(JobStateBase jobState)
    {
        StateDisplayName = jobState.GetType().GetDisplayName();
    }

    IReadOnlyList<Process> IPredictiveJob.PredictedFailures => PredictedFailures;
}