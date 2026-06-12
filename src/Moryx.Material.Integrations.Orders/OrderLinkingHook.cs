// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;
using Moryx.Orders;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// Specialized <see cref="LinkingHook"/> base class for order-linking scenarios. Provides
/// strongly typed access to the <see cref="Order"/> business object resolved by the
/// integration manager.
/// </summary>
public abstract class OrderLinkingHook : LinkingHook
{
    /// <summary>
    /// The resolved <see cref="Order"/> being linked. <c>null</c> when handling a pure unlink request.
    /// </summary>
    protected Order? Order { get; internal set; }

    /// <summary>
    /// The previously linked <see cref="Order"/>, if any. Set when re-linking auto-unlinks first.
    /// </summary>
    protected Order? PreviousOrder { get; internal set; }

    /// <summary>
    /// Strongly typed access to the order linking request.
    /// </summary>
    protected OrderLinkingRequest OrderRequest => (OrderLinkingRequest)Request;

    /// <summary>
    /// Strongly typed access to the order-linked container.
    /// </summary>
    protected new IOrderLinkedMaterialContainer Container => (IOrderLinkedMaterialContainer)base.Container;
}