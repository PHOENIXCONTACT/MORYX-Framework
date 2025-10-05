// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.ControlSystem.Jobs;
using Moryx.Threading;

namespace Moryx.Orders.Dispatcher
{
    /// <summary>
    /// Base class for all implementations of <see cref="IOperationDispatcher"/> using custom configuration
    /// </summary>
    public abstract class OperationDispatcherBase<TConf> : IOperationDispatcher
        where TConf : OperationDispatcherConfig
    {
        #region Dependencies

        /// <summary>
        /// The job management injected by the container
        /// </summary>
        public IJobManagement JobManagement { get; set; }

        /// <summary>
        /// ParallelOperations injected by the container
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>
        /// Config of this dispatcher implementation
        /// </summary>
        public TConf Config { get; set; }

        #endregion

        /// <inheritdoc />
        public void Initialize(OperationDispatcherConfig config)
        {
            Config = (TConf)config;
        }

        /// <inheritdoc />
        public virtual void Start()
        {
        }

        /// <inheritdoc />
        public virtual void Stop()
        {
        }

        /// <inheritdoc />
        public abstract void Dispatch(Operation operation, IReadOnlyList<DispatchContext> dispatchContexts);

        /// <inheritdoc />
        public abstract void Complete(Operation operation);

        /// <summary>
        /// Update method when a jobs progress has changed
        /// </summary>
        public virtual void JobProgressChanged(Operation operation, Job job)
        {
        }

        /// <summary>
        /// Update method which ensures that an operationData is present and executed with parallelOperations
        /// </summary>
        public virtual void JobStateChanged(Operation operation, JobStateChangedEventArgs eventArgs)
        {
        }

        /// <summary>
        /// Adds a job to operation data
        /// </summary>
        protected void AddJobs(Operation operation, JobCreationContext context)
        {
            var newJobs = JobManagement.Add(context);
            JobsDispatched?.Invoke(this, new JobDispatchedEventArgs(operation, newJobs));

        }

        /// <inheritdoc />
        public event EventHandler<JobDispatchedEventArgs> JobsDispatched;
    }

    /// <inheritdoc />
    /// <summary>
    /// Base class for all implementations of <see cref="T:Moryx.Orders.Dispatcher.IOperationDispatcher" />
    /// </summary>
    public abstract class OperationDispatcherBase : OperationDispatcherBase<OperationDispatcherConfig>
    {

    }
}