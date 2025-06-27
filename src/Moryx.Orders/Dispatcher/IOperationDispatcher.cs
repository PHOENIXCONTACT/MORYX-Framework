// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.ControlSystem.Jobs;
using Moryx.Modules;

namespace Moryx.Orders.Dispatcher
{
    /// <summary>
    /// Interface used dispatch jobs for operations
    /// </summary>
    public interface IOperationDispatcher : IConfiguredPlugin<OperationDispatcherConfig>
    {
        /// <summary>
        /// A new job will be added to the <see cref="IJobManagement"/> and to the given operation.
        /// The new job will be moved to the bottom of all jobs of the given operation
        /// </summary>
        void Dispatch(Operation operation, IReadOnlyList<DispatchContext> dispatchContexts);

        /// <summary>
        /// Will complete all jobs of the given operation
        /// </summary>
        void Complete(Operation operation);

        /// <summary>
        /// Update method when a jobs progress has changed
        /// </summary>
        void JobProgressChanged(Operation operation, Job job);

        /// <summary>
        /// Update method which ensures that an operationData is present and executed with parallelOperations
        /// </summary>
        void JobStateChanged(Operation operation, JobStateChangedEventArgs eventArgs);

        /// <summary>
        /// Event which will be raised if a job was dispatched
        /// </summary>
        event EventHandler<JobDispatchedEventArgs> JobsDispatched;
    }
}