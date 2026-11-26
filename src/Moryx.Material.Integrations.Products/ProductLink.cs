// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Link of a material container to a product type.
/// </summary>
/// <remarks>
/// Declare it as get-only property on a <see cref="MaterialContainer"/> implementing <see cref="IProductLinkedMaterialContainer"/>.
/// </remarks>
[DataContract]
public sealed class ProductLink : MaterialContainerLink<ProductTypeReference>
{
    /// <summary>
    /// Requests to link the product type with <paramref name="productIdentity"/>, unlinking the currently linked product type.
    /// </summary>
    /// <param name="productIdentity">String representation of the product identity (e.g. <c>PRD01-01</c>).</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    public Task<LinkingResult> LinkAsync(string productIdentity, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(productIdentity))
        {
            throw new ArgumentException("Product identity is required.", nameof(productIdentity));
        }

        return RequestAsync(new ProductTypeReference(productIdentity), cancellationToken);
    }
}
