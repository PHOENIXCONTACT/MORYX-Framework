// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// Event args raised by a container after the order link has been applied (or unlinking completed).
/// </summary>
public class OrderLinkAppliedEventArgs(OrderLinkingRequest request, ValidationContext context, OrderReference? appliedReference)
    : LinkingAppliedEventArgs(request, context)
{

    /// <summary>
    /// Strongly typed access to the order-specific request payload.
    /// </summary>
    public OrderLinkingRequest OrderRequest => (OrderLinkingRequest)Request;

    /// <summary>
    /// The reference that was applied to the container, or <c>null</c> for an unlink.
    /// </summary>
    public OrderReference? AppliedReference { get; } = appliedReference;
}
