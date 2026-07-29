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
    /// <summary>
    /// Order number; always available even when <see cref="State"/> is not <see cref="ReferenceState.Active"/>.
    /// </summary>
    [DataMember]
    public string OrderNumber { get; protected set; } = string.Empty;

    /// <summary>
    /// Operation number, null if operation is not specified
    /// </summary>
    [DataMember]
    public string? OperationNumber { get; protected set; }

    /// <summary>
    /// Cached operation status, mapped from the underlying business object when active.
    /// This information is rest when the reference is not actively maintained.
    /// </summary>
    public OperationStateClassification? Status { get; protected set; }

    /// <summary>
    /// Creates a new <see cref="OrderReference"/> in <see cref="ReferenceState.Initialized"/>.
    /// </summary>
    public OrderReference(string orderNumber, string? operationNumber = null)
    {
        OrderNumber = orderNumber ?? throw new ArgumentNullException(nameof(orderNumber));
        OperationNumber = operationNumber;
    }
}

public static class OrderReferenceExtensions
{
    extension(OrderReference? reference)
    {
        public bool ValueEquals(OrderReference? other) => (reference is null && other is null)
            || other is not null && reference is not null && reference.OrderNumber == other.OrderNumber && reference.OperationNumber == other.OperationNumber;
    }
}
