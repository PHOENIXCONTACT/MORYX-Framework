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
    public Order? Order { protected get; set; }

    /// <summary>
    /// The previously linked <see cref="Order"/>, if any. Set when re-linking auto-unlinks first.
    /// </summary>
    public Order? PreviousOrder { protected get; set; }

    // ToDo: Why doesn't this work?
    ///// <summary>
    ///// Strongly typed access to the order linking request.
    ///// </summary>
    //public new OrderLinkingRequest Request => (OrderLinkingRequest)base.Request;

    ///// <summary>
    ///// Strongly typed access to the order-linked container.
    ///// </summary>
    //public required IOrderLinkedMaterialContainer Container
    //{
    //    protected get => (IOrderLinkedMaterialContainer)base.Container;
    //    set => base.Container = value;
    //}
}
