// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.ControlSystem.Jobs;
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
                throw; // TODO handle exception
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
                Dispatcher.JobProgressChanged(operationData.Operation, job);
            }
            catch (Exception e)
            {
                throw; // TODO handle exception
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
                Dispatcher.JobStateChanged(operationData.Operation, eventArgs);
            }
            catch (Exception e)
            {
                throw; // TODO handle exception
            }
        }

        /// <inheritdoc />
        public void Dispatch(IOperationData operationData, IReadOnlyList<DispatchContext> dispatchContexts)
        {
            Dispatcher.Dispatch(operationData.Operation, dispatchContexts);
        }

        /// <inheritdoc />
        public void Complete(IOperationData operationData)
        {
            Dispatcher.Complete(operationData.Operation);
        }

        /// <inheritdoc />
        public IEnumerable<Job> Restore(IEnumerable<long> jobIds)
            => jobIds.Select(id => JobManagement.Get(id));
    }
}
