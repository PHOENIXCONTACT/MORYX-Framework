// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.Orders.Dispatcher;
using Moryx.Threading;
using Moryx.Tools;
using Moryx.AbstractionLayer.Recipes;

namespace Moryx.Orders.Management;

/// <summary>
/// Default dispatcher which will be used by the operation
/// </summary>
[Plugin(LifeCycle.Singleton, typeof(IOperationDispatcher), Name = nameof(PreallocateJobOperationDispatcher))]
public class PreallocateJobOperationDispatcher : OperationDispatcherBase
{
    /// <summary>
    /// ParallelOperations injected by the container
    /// </summary>
    public IParallelOperations ParallelOperations { get; set; }

    private readonly AllocationStore _allocationStore = new();

    /// <inheritdoc />
    public override async Task DispatchAsync(Operation operation, IReadOnlyList<DispatchContext> dispatchContexts)
    {
        var creationContext = new JobCreationContext();
        var validDispatchContexts = dispatchContexts.Where(c => c.Recipe != null && c.Amount > 0);
        foreach (var c in validDispatchContexts)
        {
            var key = new AllocationKey(operation.Identifier, c.Recipe.TemplateId);
            // Try to atomically add a new token for this operation+recipe or adjust an existing one.
            await _allocationStore.AddOrAdjustAsync(key, c.Amount, () => creationContext.Preallocate(c.Recipe, c.Amount));
        }

        // All jobs have been dispatched on existing preallocations
        if (creationContext.Templates.Count == 0)
        {
            return;
        }

        // Position after running or completing jobs of the operation if any
        var lastJob = operation.Jobs.LastOrDefault(j => j.Classification < JobClassification.Completed);
        if (lastJob != null)
        {
            creationContext.After(lastJob);
        }

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
    public override Task CompleteAsync(Operation operation)
    {
        var jobs = operation.Jobs.Where(j => j.Classification < JobClassification.Completing);
        ParallelOperations.ExecuteParallel(() => jobs.ForEach(job => JobManagement.Complete(job)));

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task StopAsync()
    {
        _allocationStore.Clear();
        return base.StopAsync();
    }

    /// <summary>
    /// Keeps exactly one active <see cref="AllocationToken"/> per <see cref="Operation.Identifier"/>
    /// and <see cref="IRecipe.TemplateId"/> in a thread-safe data structure.
    /// </summary>
    private sealed class AllocationStore
    {
        /// <summary>
        /// Stores allocation tokens organized by operation and template identifiers.
        /// </summary>
        private readonly ConcurrentDictionary<AllocationKey, AllocationToken> _store = new();

        /// <summary>
        /// Lightweight per-semaphoreKey locks to guarantee atomic check-create-or-adjust semantics with low contention
        /// </summary>
        private readonly ConcurrentDictionary<AllocationKey, SemaphoreSlim> _semaphores = new();

        /// <summary>
        /// Atomically add a new allocation token for the <paramref name="operationId"/> + <paramref name="recipeId"/>
        /// or adjust an existing one. The <paramref name="createToken"/> function is called only when a new token must be created.
        /// </summary>
        public async Task AddOrAdjustAsync(AllocationKey key, uint amount, Func<AllocationToken> createToken)
        {
            var gate = _semaphores.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            await gate.ExecuteAsync(async () =>
            {
                // If token already registered, we can adjust it otherwise we silently skip and wait for later dispatches
                if (_store.TryGetValue(key, out var existingToken) && existingToken.Status == AllocationTokenStatus.Registered)
                {
                    await existingToken.AdjustAllocationAsync((int)amount);
                }
                else
                {
                    CreatePreallocation(key, createToken);
                }
            });
        }

        private void CreatePreallocation(AllocationKey key, Func<AllocationToken> createToken)
        {
            var token = createToken();
            _store.TryAdd(key, token);
            token.StatusChanged += OnStatusChanged;
        }

        private void OnStatusChanged(object source, AllocationTokenStatus status)
        {
            if (source is not AllocationToken token || token.Status != AllocationTokenStatus.Dropped)
            {
                return;
            }

            // Drop token and clean-up all dicitonaries
            token.StatusChanged -= OnStatusChanged;
            if (TryGetKeyByValue(token, out var storeKey))
            {
                _store.TryRemove(storeKey, out _);
                _semaphores.TryRemove(storeKey, out _);
            }
        }

        private bool TryGetKeyByValue(AllocationToken value, out AllocationKey key)
        {
            foreach (var kvp in _store)
            {
                if (kvp.Value.Equals(value))
                {
                    key = kvp.Key;
                    return true;
                }
            }

            key = default;
            return false;
        }

        internal void Clear()
        {
            _store.ForEach(kvp => kvp.Value.StatusChanged -= OnStatusChanged);
            _store.Clear();
            _semaphores.Clear();
        }
    }

    private readonly record struct AllocationKey(Guid OperationId, long RecipeId);
}

