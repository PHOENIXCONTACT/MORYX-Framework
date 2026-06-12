// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Material.Linking;
using Moryx.Orders;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// Reference wrapper for an <see cref="Order"/> business object owned by the order integration.
/// </summary>
[DataContract]
public class OrderReference : Reference
{
    private Order? _order;

    /// <summary>
    /// Order number; always available even when <see cref="State"/> is not <see cref="ReferenceState.Active"/>.
    /// </summary>
    [DataMember]
    public string OrderNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Optional operation number.
    /// </summary>
    [DataMember]
    public string? OperationNumber { get; private set; }

    /// <summary>
    /// Cached operation status, mapped from the underlying business object when active.
    /// </summary>
    [DataMember]
    public string? Status { get; private set; }

    /// <summary>
    /// Creates a new <see cref="OrderReference"/> in <see cref="ReferenceState.Initialized"/>.
    /// </summary>
    public OrderReference(string orderNumber, string? operationNumber = null)
    {
        OrderNumber = orderNumber ?? throw new ArgumentNullException(nameof(orderNumber));
        OperationNumber = operationNumber;
    }

    /// <summary>
    /// Parameterless constructor for serialization frameworks.
    /// </summary>
    protected OrderReference() { }

    /// <summary>
    /// Order business object resolved by the integration. May be <c>null</c> when the
    /// reference is not <see cref="ReferenceState.Active"/>.
    /// </summary>
    public Order? Order => _order;

    /// <summary>
    /// Attaches the resolved <see cref="Order"/> business object and transitions the reference to <see cref="ReferenceState.Active"/>.
    /// Intended to be called by the integration's <c>OrderContainerManager</c>.
    /// </summary>
    internal void Attach(Order order)
    {
        _order = order ?? throw new ArgumentNullException(nameof(order));
        OrderNumber = order.Number;
        // Operation number is preserved unless not set.
        State = ReferenceState.Active;
    }

    /// <summary>
    /// Detaches the underlying <see cref="Order"/>, transitioning the reference to <see cref="ReferenceState.Inactive"/>.
    /// </summary>
    internal void Detach()
    {
        _order = null;
        State = ReferenceState.Inactive;
    }

    /// <summary>
    /// Marks this reference as unavailable (lookup failed).
    /// </summary>
    internal void MarkUnavailable()
    {
        _order = null;
        State = ReferenceState.Unavailable;
    }
}