// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;
using Moryx.Modules;

namespace Moryx.Orders.Dispatcher
{
    /// <summary>
    /// Interface used dispatch jobs for operations
    /// </summary>
    public interface IOperationDispatcher : IAsyncConfiguredPlugin<OperationDispatcherConfig>
    {
        /// <summary>
        /// A new job will be added to the <see cref="IJobManagement"/> and to the given operation.
        /// The new job will be moved to the bottom of all jobs of the given operation
        /// </summary>
        Task DispatchAsync(Operation operation, IReadOnlyList<DispatchContext> dispatchContexts, CancellationToken cancellationToken = default);

        /// <summary>
        /// Will complete all jobs of the given operation
        /// </summary>
        Task CompleteAsync(Operation operation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update method when a jobs progress has changed
        /// </summary>
        Task JobProgressChangedAsync(Operation operation, Job job, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update method which ensures that an operationData is present and executed with parallelOperations
        /// </summary>
        Task JobStateChangedAsync(Operation operation, JobStateChangedEventArgs eventArgs, CancellationToken cancellationToken = default);

        /// <summary>
        /// Event which will be raised if a job was dispatched
        /// </summary>
        event EventHandler<JobDispatchedEventArgs> JobsDispatched;
    }
}
