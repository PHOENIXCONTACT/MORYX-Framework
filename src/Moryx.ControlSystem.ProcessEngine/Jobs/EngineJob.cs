// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.Tools;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Job that includes the prediction feature
    /// </summary>
    [DebuggerDisplay(nameof(EngineJob) + " <Id: {" + nameof(Id) + "}, Recipe: {" + nameof(Recipe) + "}, Classification: {" + nameof(Classification) + "}>")]
    internal class EngineJob : Job, IPredictiveJob
    {
        public List<IProcess> Running { get; set; }

        public List<IProcess> PredictedFailures { get; set; }

        public List<IProcess> TotalProcesses { get; set; }

        public EngineJob(IRecipe recipe, int amount) : base(recipe, amount)
        {
            // Prepare both process lists
            PredictedFailures = new ProcessList<IProcess>();
            RunningProcesses = Running = new ProcessList<IProcess>(32);
            AllProcesses = TotalProcesses = new ProcessList<IProcess>(amount);
        }

        public void UpdateRecipe(IRecipe recipe) => Recipe = recipe;

        public void UpdateStateDisplayName(JobStateBase jobState)
        {
            StateDisplayName = jobState.GetType().GetDisplayName();
        }

        IReadOnlyList<IProcess> IPredictiveJob.PredictedFailures => PredictedFailures;
    }
}
