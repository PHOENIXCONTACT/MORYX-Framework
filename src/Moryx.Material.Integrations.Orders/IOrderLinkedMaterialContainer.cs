// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// Material container with the ability to be linked to an order.
/// </summary>
public interface IOrderLinkedMaterialContainer : IMaterialContainer
{
    /// <summary>
    /// Currently linked order, if any.
    /// </summary>
    OrderReference? LinkedOrder { get; set; }

    /// <summary>
    /// Initiates a linking request, optionally auto-unlinking the previously linked order.
    /// </summary>
    Task RequestOrderLinkAsync(string orderNumber, string? operationNumber = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates an unlinking request to detach the currently linked order.
    /// </summary>
    Task RequestOrderUnlinkAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Raised when a link or unlink request is made by the container.
    /// Listeners (typically <c>LinkingHookManager</c>) execute hooks and return a response
    /// via <see cref="LinkingRequestEventArgs.ResponseCallback"/>.
    /// </summary>
    event EventHandler<OrderLinkRequestEventArgs>? OrderLinkRequested;

    /// <summary>
    /// Raised by the container after a link has been applied (or unlinking completed).
    /// </summary>
    event EventHandler<OrderLinkAppliedEventArgs>? OrderLinkApplied;
}
