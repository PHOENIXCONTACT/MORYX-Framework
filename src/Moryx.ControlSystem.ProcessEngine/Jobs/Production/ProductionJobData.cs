// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.StateMachines;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Production
{
    /// <summary>
    /// Implementation of a production job
    /// </summary>
    [DebuggerDisplay(nameof(ProductionJobData) + " <Id: {" + nameof(Id) + "}, State: {" + nameof(State) + "}>")]
    [Component(LifeCycle.Transient, typeof(IProductionJobData))]
    internal class ProductionJobData : JobDataBase, IProductionJobData
    {
        #region Fields and Properties

        /// <inheritdoc />
        public new ProductionRecipe Recipe => (ProductionRecipe)base.Recipe;

        /// <inheritdoc cref="IJobData"/>
        public int SuccessCount { get; private set; }

        /// <inheritdoc cref="IProductionJobData"/>
        public int FailureCount { get; private set; }

        /// <inheritdoc />
        public int PredictedFailures => _predictedFailures.Count;

        /// <inheritdoc cref="IProductionJobData"/>
        public int ReworkedCount { get; private set; }

        /// <inheritdoc cref="IProductionJobData"/>
        public int ProcessCount => RunningProcesses.Count + SuccessCount + FailureCount;

        /// <summary>
        /// <c>True</c> if <see cref="ProcessCount"/> is equal to the Amount/>
        /// </summary>
        internal bool AmountReached => ProcessCount == Amount;

        /// <summary>
        /// Processes that are predicted to fail soon
        /// </summary>
        private readonly ICollection<ProcessData> _predictedFailures = new List<ProcessData>();

        /// <summary>
        /// Private casted property for the <see cref="ProductionJobStateBase"/>
        /// </summary>
        private new ProductionJobStateBase State => (ProductionJobStateBase)base.State;

        #endregion

        /// <inheritdoc />
        public ProductionJobData(ProductionRecipe recipe, int amount) : base(recipe, amount)
        {
            StateMachine.Initialize(this).With<ProductionJobStateBase>();
        }

        /// <inheritdoc />
        public ProductionJobData(ProductionRecipe recipe, JobEntity entity) : base(recipe, entity)
        {
            Started = entity.Created;

            StateMachine.Reload(this, entity.State).With<ProductionJobStateBase>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Restore counters from AllProcesses
        /// </summary>
        internal override void LoadProcesses()
        {
            base.LoadProcesses();

            Job.SuccessCount = SuccessCount = AllProcesses.Count(p => p.State == ProcessState.Success);
            Job.FailureCount = FailureCount = AllProcesses.Count(p => p.State == ProcessState.Failure);
            Job.ReworkedCount = ReworkedCount = AllProcesses.Count(p => p.Rework && (p.State == ProcessState.Success || p.State == ProcessState.Failure));
        }

        /// <inheritdoc />
        public override void FailurePredicted(ProcessData processData)
        {
            lock (_predictedFailures)
                _predictedFailures.Add(processData);
            Job.PredictedFailures.Add(processData.Process);

            RaiseProgressChanged();
        }

        /// <summary>
        /// Latest process is running
        /// </summary>
        internal void ProcessRunning(ProcessData processData)
        {
            Job.Running.Add(processData.Process);
            RaiseProgressChanged();
        }

        /// <inheritdoc />
        internal override void ProcessCompleted(ProcessData processData)
        {
            RunningProcesses.Remove(processData);
            Job.Running.Remove(processData.Process);

            lock (_predictedFailures)
                _predictedFailures.Remove(processData);

            Job.PredictedFailures.Remove(processData.Process);

            // Count the reworked process which could be also a success or failure process
            if (processData.Rework && (processData.State == ProcessState.Success || processData.State == ProcessState.Failure))
                Job.ReworkedCount = ++ReworkedCount;

            if (processData.State == ProcessState.Discarded)
                AllProcesses.Remove(processData); // Discarded processes are removed from all processes as well
            else if (processData.State == ProcessState.Success)
                Job.SuccessCount = ++SuccessCount;
            else if (processData.State == ProcessState.Failure)
                Job.FailureCount = ++FailureCount;
            else
                return;

            base.ProcessCompleted(processData);
        }

        /// <summary>
        /// Used by the state machine.
        /// Removes created processes on the <see cref="IJobDispatcher"/>
        /// Will not abort running. Running processes will be finished
        /// </summary>
        internal void DiscardCachedProcess()
        {
            Dispatcher.Complete(this);
        }

        /// <summary>
        /// Check if the process is the last process which was added to the job
        /// </summary>
        internal bool IsLatestProcess(ProcessData processData)
        {
            return RunningProcesses.Last().Equals(processData);
        }
    }
}
