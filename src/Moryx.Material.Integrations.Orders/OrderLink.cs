// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// Link of a material container to an order (operation).
/// </summary>
/// <remarks>
/// Declare it as get-only property on a <see cref="MaterialContainer"/> implementing <see cref="IOrderLinkedMaterialContainer"/>.
/// </remarks>
[DataContract]
public sealed class OrderLink : MaterialContainerLink<OrderReference>
{
    /// <summary>
    /// Requests to link the order with <paramref name="orderNumber"/> and <paramref name="operationNumber"/>,
    /// unlinking the currently linked order.
    /// </summary>
    public Task<LinkingResult> LinkAsync(string orderNumber, string? operationNumber = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(orderNumber))
        {
            throw new ArgumentException("Order number is required.", nameof(orderNumber));
        }

        return RequestAsync(new OrderReference(orderNumber, operationNumber), cancellationToken);
    }
}
