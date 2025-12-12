// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.Orders.Dispatcher;
using Moryx.Threading;
using Moryx.Tools;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Default dispatcher which will be used by the operation
    /// </summary>
    [Plugin(LifeCycle.Singleton, typeof(IOperationDispatcher), Name = nameof(SingleJobOperationDispatcher))]
    public class SingleJobOperationDispatcher : OperationDispatcherBase
    {
        /// <summary>
        /// ParallelOperations injected by the container
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        /// <inheritdoc />
        public override async Task DispatchAsync(Operation operation, IReadOnlyList<DispatchContext> dispatchContexts,
            CancellationToken cancellationToken)
        {
            // Wait until all jobs are completing to avoid separate jobs for every scrap
            var allCompleting = operation.Jobs.All(j => j.Classification >= JobClassification.Completing);
            if (!allCompleting)
                return;

            var lastJob = operation.Jobs.LastOrDefault(j => j.Classification < JobClassification.Completed);

            var creationContext = new JobCreationContext();
            dispatchContexts.ForEach(c => creationContext.Add(c.Recipe, c.Amount));

            if (lastJob != null)
                creationContext.After(lastJob);

            try
            {
                await AddJobsAsync(operation, creationContext);
            }
            catch (KeyNotFoundException)
            {
                // Positioning failed, because reference already completed
                creationContext.Append();
                await AddJobsAsync(operation, creationContext);
            }
        }

        /// <inheritdoc />
        public override Task CompleteAsync(Operation operation, CancellationToken cancellationToken)
        {
            var jobs = operation.Jobs.Where(j => j.Classification < JobClassification.Completing);
            ParallelOperations.ExecuteParallel(() => jobs.ForEach(job => JobManagement.Complete(job)));

            return Task.CompletedTask;
        }
    }
}
