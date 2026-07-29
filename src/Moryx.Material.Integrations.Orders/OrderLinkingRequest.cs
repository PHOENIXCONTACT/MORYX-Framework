// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// Order-specific <see cref="LinkingRequest"/> payload.
/// </summary>
public class OrderLinkingRequest : LinkingRequest
{
    /// <summary>
    /// Order number being linked. Empty if this is a pure unlinking request.
    /// </summary>
    public string? OrderNumber { get; }

    /// <summary>
    /// Optional operation number.
    /// </summary>
    public string? OperationNumber { get; }

    /// <summary>
    /// Previously linked order reference, if any. Set when re-linking with auto-unlink.
    /// </summary>
    public OrderReference? PreviousOrder { get; }

    /// <summary>
    /// Creates a linking (or re-linking) request.
    /// </summary>
    public OrderLinkingRequest(string orderNumber, string? operationNumber = null, OrderReference? previousOrder = null)
    {
        OrderNumber = orderNumber ?? throw new ArgumentNullException(nameof(orderNumber));
        OperationNumber = operationNumber;
        PreviousOrder = previousOrder;
        IsUnlink = false;
    }

    // TODO: Add nice extension methods on LinkingRequest for creating the object
    /// <summary>
    /// Creates a pure unlinking request.
    /// </summary>
    public OrderLinkingRequest(OrderReference previousOrder)
    {
        PreviousOrder = previousOrder ?? throw new ArgumentNullException(nameof(previousOrder));
        IsUnlink = true;
    }
}
