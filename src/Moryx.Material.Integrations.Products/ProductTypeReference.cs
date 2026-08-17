// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Products;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Reference wrapper for a <see cref="ProductType"/> business object owned by the product integration.
/// </summary>
[DataContract]
public class ProductTypeReference : Reference
{
    /// <summary>
    /// Product identity in its string representation (identifier + revision); always available
    /// even when the reference is not <see cref="ReferenceState.Active"/>.
    /// </summary>
    [DataMember]
    public string ProductIdentity { get; protected set; } = string.Empty;

    /// <summary>
    /// Cached display name of the product type, mapped from the underlying business object when active.
    /// This information is reset when the reference is not actively maintained.
    /// </summary>
    public string? Name { get; protected set; }

    /// <summary>
    /// Cached product state, mapped from the underlying business object when active.
    /// This information is reset when the reference is not actively maintained.
    /// </summary>
    public ProductState? Status { get; protected set; }

    /// <summary>
    /// Creates a new instance of a <see cref="ProductInstance"/> for the referenced
    /// <see cref="ProductType"/> when this reference is <see cref="ReferenceState.Active"/>.
    /// Returns <c>null</c> otherwise.
    /// </summary>
    public virtual ProductInstance? CreateInstance() => null;

    /// <summary>
    /// Creates a new <see cref="ProductTypeReference"/> in <see cref="ReferenceState.Initialized"/>.
    /// </summary>
    public ProductTypeReference(string productIdentity)
    {
        ProductIdentity = productIdentity ?? throw new ArgumentNullException(nameof(productIdentity));
    }
}

/// <summary>
/// Convenience extensions on <see cref="ProductTypeReference"/>.
/// </summary>
public static class ProductTypeReferenceExtensions
{
    extension(ProductTypeReference? reference)
    {
        /// <summary>
        /// Value-based equality check comparing the product identity of both references.
        /// </summary>
        public bool ValueEquals(ProductTypeReference? other) => (reference is null && other is null)
            || other is not null && reference is not null && reference.ProductIdentity == other.ProductIdentity;
    }
}