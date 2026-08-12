// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;
using Moryx.Orders;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

/// <summary>
/// Lifecycle pool for managed <see cref="InternalOrderReference"/> instances synchronized with orders in the <see cref="IOrderManagement"/>.
/// </summary>
internal interface IOrderReferencesPool : IAsyncPlugin
{
    /// <summary>
    /// Gets the managed reference matching <paramref name="reference"/>, if one exists.
    /// </summary>
    InternalOrderReference? Get(OrderReference? reference);

    InternalOrderReference? GetOrCreate(string orderNumber, string? operationNumber);
}
