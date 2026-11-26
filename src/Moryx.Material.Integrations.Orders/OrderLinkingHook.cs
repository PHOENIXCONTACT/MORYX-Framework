// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;
using Moryx.Orders;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// Specialized <see cref="LinkingHook{TReference, TBusinessObject}"/> base class for order-linking scenarios.
/// Provides strongly typed access to the <see cref="Order"/> business objects resolved by the order integration
/// through <see cref="LinkingHook{TReference, TBusinessObject}.Target"/> and
/// <see cref="LinkingHook{TReference, TBusinessObject}.Previous"/>.
/// </summary>
public abstract class OrderLinkingHook : LinkingHook<OrderReference, Order>
{
}
