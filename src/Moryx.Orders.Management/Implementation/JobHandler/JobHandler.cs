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

        private void OnJobsDispatched(object sender, JobDispatchedEventArgs e)
        {
            var operationData = OperationDataPool.Get(e.Operation);

            foreach (var addedJob in e.Jobs)
                operationData.AddJob(addedJob);
        }

        private void OnJobProgressChanged(object sender, Job job)
        {
            var operationData = OperationDataPool.GetAll(op => op.Operation.Jobs.Any(j => j.Id == job.Id)).FirstOrDefault();

            // If operation was found and currently not dispatching, we can throw updates
            if (operationData == null)
                return;

            operationData.JobProgressChanged(job);
            Dispatcher.JobProgressChanged(operationData.Operation, job);
        }

        /// <summary>
        /// If a job was updated, the operation will be informed.
        /// If no operation was found on the pool which refers to the job,
        /// the update will be ignored.
        /// </summary>
        private void OnJobStateChanged(object sender, JobStateChangedEventArgs eventArgs)
        {
            var operationData = OperationDataPool.GetAll(op => op.Operation.Jobs.Any(job => job == eventArgs.Job)).FirstOrDefault();

            // If operation was found and currently not dispatching, we can throw updates
            if (operationData == null)
                return;

            operationData.JobStateChanged(eventArgs);
            Dispatcher.JobStateChanged(operationData.Operation, eventArgs);
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
