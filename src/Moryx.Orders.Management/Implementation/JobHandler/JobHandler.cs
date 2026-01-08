// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.Logging;
using Moryx.Orders.Dispatcher;
using Moryx.Threading;

namespace Moryx.Orders.Management
{
    [Component(LifeCycle.Singleton, typeof(IJobHandler))]
    internal class JobHandler : IJobHandler
    {
        /// <summary>
        /// The job management injected by the container
        /// </summary>
        public IJobManagement JobManagement { get; set; }

        /// <summary>
        /// The OperationPool injected by the container
        /// </summary>
        public IOperationDataPool OperationDataPool { get; set; }

        /// <summary>
        /// ParallelOperations injected by the container
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>
        /// Operation dispatcher will be used to execute the dispatch of a job
        /// </summary>
        public IOperationDispatcher Dispatcher { get; set; }

        /// <summary>
        /// Logger for this component
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <inheritdoc />
        public void Start()
        {
            JobManagement.ProgressChanged += ParallelOperations.DecoupleListener<Job>(OnJobProgressChanged);
            JobManagement.StateChanged += ParallelOperations.DecoupleListener<JobStateChangedEventArgs>(OnJobStateChanged);

            Dispatcher.JobsDispatched += OnJobsDispatched;
        }

        /// <inheritdoc />
        public void Stop()
        {
            Dispatcher.JobsDispatched -= OnJobsDispatched;

            JobManagement.StateChanged -= ParallelOperations.RemoveListener<JobStateChangedEventArgs>(OnJobStateChanged);
            JobManagement.ProgressChanged -= ParallelOperations.RemoveListener<Job>(OnJobProgressChanged);
        }

        private async void OnJobsDispatched(object sender, JobDispatchedEventArgs eventArgs)
        {
            try
            {
                var operationData = OperationDataPool.Get(eventArgs.Operation);

                foreach (var addedJob in eventArgs.Jobs)
                    await operationData.AddJob(addedJob);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during adding jobs to operation {operationIdentifier}.", eventArgs.Operation.Identifier);
            }
        }

        private async void OnJobProgressChanged(object sender, Job job)
        {
            try
            {
                var operationData = OperationDataPool.GetAll(op => op.Operation.Jobs.Any(j => j.Id == job.Id)).FirstOrDefault();

                // If operation was found and currently not dispatching, we can throw updates
                if (operationData == null)
                    return;

                await operationData.JobProgressChanged(job);
                await Dispatcher.JobProgressChangedAsync(operationData.Operation, job, CancellationToken.None);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during job progress change of job {jobId}.", job.Id);
            }
        }

        /// <summary>
        /// If a job was updated, the operation will be informed.
        /// If no operation was found on the pool which refers to the job,
        /// the update will be ignored.
        /// </summary>
        private async void OnJobStateChanged(object sender, JobStateChangedEventArgs eventArgs)
        {
            try
            {
                var operationData = OperationDataPool.GetAll(op => op.Operation.Jobs.Any(job => job == eventArgs.Job)).FirstOrDefault();

                // If operation was found and currently not dispatching, we can throw updates
                if (operationData == null)
                    return;

                await operationData.JobStateChanged(eventArgs);
                await Dispatcher.JobStateChangedAsync(operationData.Operation, eventArgs, CancellationToken.None);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during job state change of job {jobId}.", eventArgs.Job.Id);
            }
        }

        /// <inheritdoc />
        public Task Dispatch(IOperationData operationData, IReadOnlyList<DispatchContext> dispatchContexts)
        {
            return Dispatcher.DispatchAsync(operationData.Operation, dispatchContexts, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task Complete(IOperationData operationData)
        {
            return Dispatcher.CompleteAsync(operationData.Operation, CancellationToken.None);
        }

        /// <inheritdoc />
        public IEnumerable<Job> Restore(IEnumerable<long> jobIds)
            => jobIds.Select(id => JobManagement.Get(id));
    }
}
