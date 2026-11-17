// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using Moryx.Container;
using Moryx.Material.Linking;
using Moryx.Orders;
using Moryx.Tools;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

// ToDo: Add logic for references only using an order number without operation number
[Component(LifeCycle.Singleton, typeof(IOrderReferencesPool))]
internal class OrderReferencesPool : IOrderReferencesPool
{
    private readonly ConcurrentDictionary<Guid, InternalOrderReference> _operationReferences = new();

    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IOrderManagement OrderManagement { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    #region Lifecycle
    public Task StartAsync(CancellationToken cancellationToken)
    {
        OrderManagement.OperationUpdated += OnOperationUpdated;
        OrderManagement.OperationCompleted += OnOperationCompleted;

        OrderManagement.GetOperations(o => o.State < OperationStateClassification.Completed)
            .ForEach(SynchronizeReferenceOf);

        return Task.CompletedTask;
    }

    private void OnOperationUpdated(object? sender, OperationChangedEventArgs e)
    {
        SynchronizeReferenceOf(e.Operation);
    }

    private void SynchronizeReferenceOf(Operation operation)
    {
        _ = _operationReferences.AddOrUpdate(operation.Identifier, operation.ToReference(), (key, current) =>
        {
            current.Status = operation.State;
            current.State = ReferenceState.Active;
            return current;
        });
    }

    private void OnOperationCompleted(object? sender, OperationReportEventArgs e)
    {
        _ = _operationReferences.TryRemove(e.Operation.Identifier, out var completedOperation);
        _ = (completedOperation?.State = ReferenceState.Inactive);
        _ = (completedOperation?.Status = e.Operation.State);
    }

    private static void Deactivate(KeyValuePair<Guid, InternalOrderReference> pair)
    {
        var operation = pair.Value;
        operation.State = ReferenceState.Inactive;
        operation.Status = null;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        OrderManagement.OperationUpdated -= OnOperationUpdated;
        OrderManagement.OperationCompleted -= OnOperationCompleted;

        _operationReferences.Where(r => r.Value.IsValid()).ForEach(Deactivate);
        _operationReferences.Clear();

        return Task.CompletedTask;
    }
    #endregion

    #region IOrderReferencesPool

    public InternalOrderReference? Get(OrderReference? reference)
    {
        if (reference is null)
        {
            return null;
        }

        return _operationReferences
            .SingleOrDefault(pair => pair.Value.OrderNumber == reference.OrderNumber && pair.Value.OperationNumber == reference.OperationNumber)
            .Value;
    }

    public InternalOrderReference GetOrCreate(string orderNumber, string? operationNumber = null)
    {
        var existing = _operationReferences.SingleOrDefault(pair =>
            pair.Value.OrderNumber == orderNumber && pair.Value.OperationNumber == operationNumber,
            new KeyValuePair<Guid, InternalOrderReference>(Guid.NewGuid(), default)).Key;

        return _operationReferences.GetOrAdd(existing, guid => new(orderNumber, operationNumber)
        {
            State = ReferenceState.Unavailable
        });
    }

    public IReadOnlyList<OrderReference> GetAll() => [.. _operationReferences.Values];

    #endregion
}
