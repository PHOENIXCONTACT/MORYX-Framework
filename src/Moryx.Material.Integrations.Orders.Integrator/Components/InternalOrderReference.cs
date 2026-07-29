// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Moryx.Material.Linking;
using Moryx.Orders;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

internal class InternalOrderReference : OrderReference
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new required string OrderNumber
    {
        get;
        set => base.OrderNumber = field = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new string? OperationNumber
    {
        get;
        set => base.OperationNumber = field = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new OperationStateClassification? Status
    {
        get;
        set => base.Status = field = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new ReferenceState State
    {
        get;
        set => base.State = field = value;
    }

    [SetsRequiredMembers]
    internal InternalOrderReference(string orderNumber, string? operationNumber = null) : base(orderNumber, operationNumber)
    {
        OrderNumber = orderNumber;
    }
}

internal static class ReferenceExtensions
{
    extension(Operation operation)
    {
        public InternalOrderReference ToReference() => new(operation.Order.Number, operation.Number)
        {
            Status = operation.State,
            State = ReferenceState.Active
        };
    }

    extension(IOrderManagement orderManagement)
    {
        public async Task<Order?> LoadOrderFor(OrderReference? reference) => reference is null ? null :
            (await orderManagement.LoadOperationAsync(reference.OrderNumber, reference.OperationNumber))?.Order;

        public async Task<Order?> LoadOrderFor(string orderNumber, string? operationNumber) =>
            (await orderManagement.LoadOperationAsync(orderNumber, operationNumber))?.Order;
    }
}
