// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;
using Moryx.Modules;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Interface used by the <see cref="OperationData"/>
    /// to handle jobs. Will be used to dispatch new jobs, complete and restore them
    /// </summary>
    internal interface IJobHandler : IPlugin
    {
        /// <summary>
        /// A new job will be added to the <see cref="IJobManagement"/> and to the given operation.
        /// The new job will be moved to the bottom of all jobs of the given operation
        /// </summary>
        void Dispatch(IOperationData operationData, IReadOnlyList<DispatchContext> dispatchContexts);

        /// <summary>
        /// Will complete all jobs of the given operation
        /// </summary>
        void Complete(IOperationData operationData);

        /// <summary>
        /// Will load all given jobs by their id from the <see cref="IJobManagement"/>
        /// </summary>
        IEnumerable<Job> Restore(IEnumerable<long> jobIds);
    }
}
