// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// Event args raised when a container requests an order (un)link.
/// </summary>
public class OrderLinkRequestEventArgs : LinkingRequestEventArgs
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public OrderLinkRequestEventArgs(IOrderLinkedMaterialContainer container, OrderLinkingRequest request)
        : base(container, request)
    {
    }

    /// <summary>
    /// Strongly typed access to the order-specific request payload.
    /// </summary>
    public OrderLinkingRequest OrderRequest => (OrderLinkingRequest)Request;
}