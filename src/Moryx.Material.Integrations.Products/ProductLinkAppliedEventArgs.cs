// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Event args raised by a container after the product link has been applied (or unlinking completed).
/// </summary>
public class ProductLinkAppliedEventArgs(ProductLinkingRequest request, ValidationContext context, ProductTypeReference? appliedReference)
    : LinkingAppliedEventArgs(request, context)
{
    /// <summary>
    /// Strongly typed access to the product-specific request payload.
    /// </summary>
    public ProductLinkingRequest ProductRequest => (ProductLinkingRequest)Request;

    /// <summary>
    /// The reference that was applied to the container, or <c>null</c> for an unlink.
    /// </summary>
    public ProductTypeReference? AppliedReference { get; } = appliedReference;
}