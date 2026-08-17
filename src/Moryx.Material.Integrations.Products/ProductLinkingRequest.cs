// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Product-specific <see cref="LinkingRequest"/> payload.
/// </summary>
public class ProductLinkingRequest : LinkingRequest
{
    /// <summary>
    /// Identity of the product being linked. <c>null</c> if this is a pure unlinking request.
    /// </summary>
    public string? ProductIdentity { get; }

    /// <summary>
    /// Previously linked product reference, if any. Set when re-linking with auto-unlink.
    /// </summary>
    public ProductTypeReference? PreviousProduct { get; }

    /// <summary>
    /// Creates a linking (or re-linking) request.
    /// </summary>
    public ProductLinkingRequest(string productIdentity, ProductTypeReference? previousProduct = null)
    {
        ProductIdentity = productIdentity ?? throw new ArgumentNullException(nameof(productIdentity));
        PreviousProduct = previousProduct;
        IsUnlink = false;
    }

    /// <summary>
    /// Creates a pure unlinking request.
    /// </summary>
    public ProductLinkingRequest(ProductTypeReference previousProduct)
    {
        PreviousProduct = previousProduct ?? throw new ArgumentNullException(nameof(previousProduct));
        IsUnlink = true;
    }
}