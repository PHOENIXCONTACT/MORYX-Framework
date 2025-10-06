// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs.Setup;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.Logging;
using Moryx.Runtime.Modules;
using Moryx.Threading;
using Moryx.Tools;

namespace Moryx.ControlSystem.ProcessEngine
{
    internal class JobManagementFacade : IJobManagement, IFacadeControl
    {
        public Action ValidateHealthState { get; set; }

        #region Dependencies

        public IJobDataList JobList { get; set; }

        public IJobManager JobManager { get; set; }

        public IJobHistory History { get; set; }

        public ISetupJobHandler SetupHandler { get; set; }

        public IResourceAssignment ResourceAssignment { get; set; }

        public IModuleLogger Logger { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        #endregion

        private ParallelOperationsQueue<JobStateChangedEventArgs> _stateDecoupler;
        private ParallelOperationsQueue<IJobData> _progressDecoupler;
        private ParallelOperationsQueue<ICapabilities> _capabilitiesDecoupler;
        private ParallelOperationsQueue<IReadOnlyList<IJobData>> _addedDecoupler;

        /// <inheritdoc cref="IFacadeControl"/>
        public void Activate()
        {
            _stateDecoupler = new ParallelOperationsQueue<JobStateChangedEventArgs>(RaiseStateChanged, ParallelOperations, Logger);
            JobList.StateChanged += HandleJobStateChanged;

            _progressDecoupler = new ParallelOperationsQueue<IJobData>(RaiseJobUpdated, ParallelOperations, Logger);
            JobList.ProgressChanged += HandleJobUpdated;

            _addedDecoupler = new ParallelOperationsQueue<IReadOnlyList<IJobData>>(RaiseJobsAdded, ParallelOperations, Logger);
            JobList.Added += HandleJobAdded;

            _capabilitiesDecoupler = new ParallelOperationsQueue<ICapabilities>(RaiseEvaluationsOutdated, ParallelOperations, Logger);
            ResourceAssignment.CapabilitiesChanged += HandleCapabilitiesChanged;
        }

        /// <inheritdoc cref="IFacadeControl"/>
        public void Deactivate()
        {
            JobList.StateChanged -= HandleJobStateChanged;
            JobList.ProgressChanged -= HandleJobUpdated;
            JobList.Added -= HandleJobAdded;
            ResourceAssignment.CapabilitiesChanged -= HandleCapabilitiesChanged;
        }

        private void HandleJobStateChanged(object sender, JobStateEventArgs args)
        {
            _stateDecoupler.Enqueue(new JobStateChangedEventArgs(args.JobData.Job, args.PreviousState.Classification, args.CurrentState.Classification));
        }

        private void RaiseStateChanged(JobStateChangedEventArgs args)
        {
            StateChanged?.Invoke(this, args);
        }

        private void HandleJobUpdated(object sender, IJobData jobData)
        {
            _progressDecoupler.Enqueue(jobData);
        }

        private void RaiseJobUpdated(IJobData jobData)
        {
            ProgressChanged?.Invoke(this, jobData.Job);
        }

        private void HandleJobAdded(object sender, IReadOnlyList<IJobData> jobDatas)
        {
            _addedDecoupler.Enqueue(jobDatas);
        }

        private void RaiseJobsAdded(IReadOnlyList<IJobData> jobDatas)
        {
            foreach (var jobData in jobDatas)
            {
                var args = new JobStateChangedEventArgs(jobData.Job, JobClassification.Idle, jobData.State.Classification);
                _stateDecoupler.Enqueue(args);
            }
        }

        public JobEvaluation Evaluate(IProductRecipe recipe, int amount)
        {
            ValidateHealthState();
            if (recipe is not IProductionRecipe prodRecipe)
                throw new ArgumentException($"Process engine only supports {nameof(ProductionRecipe)}", nameof(recipe));

            return new JobEvaluation
            {
                WorkplanErrors = WorkplanValidation.Validate(prodRecipe.Workplan)
            };
        }

        public JobEvaluation Evaluate(IProductRecipe recipe, int amount, IResourceManagement resourceManagement) => Evaluate(recipe, amount);

        private void HandleCapabilitiesChanged(object sender, ICapabilities e)
        {
            _capabilitiesDecoupler.Enqueue(e);
        }

        private void RaiseEvaluationsOutdated(ICapabilities obj)
        {
            EvaluationsOutdated?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public IReadOnlyList<Job> Add(JobCreationContext context)
        {
            if (!JobManager.AcceptingExternalJobs)
                throw new HealthStateException(ServerModuleState.Stopping);

            context.Templates.ForEach(t => ValidateRecipe(t.Recipe));

            // Move to JobManager
            var jobDatas = JobManager.Add(context);

            return jobDatas.Select(jd => jd.Job).ToList();
        }

        /// <inheritdoc cref="IJobManagement"/>
        public Job Get(long jobId)
        {
            ValidateHealthState();
            return JobList.Get(jobId)?.Job
                ?? History.Get(jobId)
                ?? throw new ArgumentException("Job does not refer to managed job", nameof(jobId));
        }

        public IReadOnlyList<Job> GetAll()
        {
            ValidateHealthState();

            return JobList.Select(j => j.Job).ToArray();
        }

        /// <inheritdoc cref="IJobManagement"/>
        public void Complete(Job job) => ValidateAndGet(job).Complete();

        /// <inheritdoc cref="IJobManagement"/>
        public void Abort(Job job) => ValidateAndGet(job).Abort();

        /// <summary>
        /// Validates the given recipe to be valid on a new job
        /// </summary>
        private void ValidateRecipe(IProductRecipe recipe)
        {
            ValidateHealthState();

            ArgumentNullException.ThrowIfNull(recipe);

            if (recipe.Origin == null)
                throw new ArgumentException("Origin must not be null on recipe", nameof(recipe));

            var productionRecipe = recipe as IProductionRecipe
                ?? throw new NotSupportedException("Process engine only supports 'IProductionRecipe'!");

            var errors = WorkplanValidation.Validate(productionRecipe.Workplan);
            if (errors.Count > 0)
            {
                var msg = string.Join("\n", errors);
                throw new ValidationException("Workplan validation failed! Errors \n" + msg);
            }
        }

        /// <summary>
        /// Will get a job from the job list.
        /// </summary>
        private IJobData ValidateAndGet(Job job)
        {
            ArgumentNullException.ThrowIfNull(job);

            ValidateHealthState();
            return JobList.Get(job.Id)
                ?? throw new ArgumentException("Job does not refer to managed job", nameof(job));
        }

        /// <inheritdoc />
        public event EventHandler<Job> ProgressChanged;

        /// <inheritdoc />
        public event EventHandler EvaluationsOutdated;

        /// <inheritdoc />
        public event EventHandler<JobStateChangedEventArgs> StateChanged;
    }
}
