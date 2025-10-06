// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.Recipes;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Components
{
    internal class BrokenJobState : IJobState
    {
        public int Key => -1;

        public JobClassification Classification => JobClassification.Completed;
    }

    internal class BrokenJobData : IProductionJobData, ISetupJobData
    {
        public BrokenJobData() => State = new BrokenJobState();

        public long Id { get; set; }

        public int Amount { get; set; }

        public JobClassification Classification => State.Classification;

        public IJobState State { get; private set; }

        public IRecipeProvider RecipeProvider { get; set; }

        public void Load() { }

        #region Not Implemented

        public IProductionRecipe Recipe => throw new NotImplementedException();

        public int SuccessCount => throw new NotImplementedException();

        public int FailureCount => throw new NotImplementedException();

        public int PredictedFailures => throw new NotImplementedException();

        public int ReworkedCount => throw new NotImplementedException();

        public int ProcessCount => throw new NotImplementedException();

        public DateTime Started => throw new NotImplementedException();

        public DateTime Completed => throw new NotImplementedException();

        public EngineJob Job => throw new NotImplementedException();

        public IReadOnlyList<ProcessData> RunningProcesses => throw new NotImplementedException();

        public IReadOnlyList<ProcessData> AllProcesses => throw new NotImplementedException();

        public bool CanComplete => throw new NotImplementedException();

        public bool CanAbort => throw new NotImplementedException();
        
        public bool IsStable => throw new NotImplementedException();

        public int RunningCount => throw new NotImplementedException();

        public int CompletedCount => throw new NotImplementedException();

        public bool RecipeRequired => throw new NotImplementedException();

        public ProcessData ActiveProcess => throw new NotImplementedException();

        IWorkplanRecipe IJobData.Recipe => throw new NotImplementedException();

        ISetupRecipe ISetupJobData.Recipe => throw new NotImplementedException();

        public event EventHandler ProgressChanged;
        public event EventHandler<JobStateEventArgs> StateChanged;

        public void Abort() => throw new NotImplementedException();

        public void AddProcess(ProcessData processData) => throw new NotImplementedException();

        public void AddProcesses(IReadOnlyList<ProcessData> processes) => throw new NotImplementedException();

        public void Complete() => throw new NotImplementedException();

        public void FailurePredicted(ProcessData processData) => throw new NotImplementedException();

        public void Interrupt() => throw new NotImplementedException();

        public void ProcessChanged(ProcessData processData, ProcessState trigger) => throw new NotImplementedException();

        public void Ready() => throw new NotImplementedException();

        public void Start() => throw new NotImplementedException();

        public void Stop() => throw new NotImplementedException();

        public void UpdateSetup(ISetupRecipe updatedRecipe) => throw new NotImplementedException();

        #endregion
    }
}

