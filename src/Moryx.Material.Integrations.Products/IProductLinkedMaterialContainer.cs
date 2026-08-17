// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Material container with the ability to be linked to a <see cref="ProductType"/>.
/// </summary>
public interface IProductLinkedMaterialContainer : IMaterialContainer
{
    /// <summary>
    /// Currently linked product type, if any.
    /// </summary>
    ProductTypeReference? LinkedProductType { get; set; }

    /// <summary>
    /// Initiates a linking request, optionally auto-unlinking the previously linked product.
    /// </summary>
    /// <param name="productIdentity">String representation of the product identity (e.g. <c>PRD01-01</c>).</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    Task RequestProductLinkAsync(string productIdentity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates an unlinking request to detach the currently linked product.
    /// </summary>
    Task RequestProductUnlinkAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Raised when a link or unlink request is made by the container.
    /// Listeners (typically <c>LinkingHookManager</c>) execute hooks and return a response
    /// via <see cref="LinkingRequestEventArgs.ResponseCallback"/>.
    /// </summary>
    event EventHandler<ProductLinkRequestEventArgs>? ProductLinkRequested;

    /// <summary>
    /// Raised by the container after a link has been applied (or unlinking completed).
    /// </summary>
    event EventHandler<ProductLinkAppliedEventArgs>? ProductLinkApplied;
}