// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Products;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Reference wrapper for a <see cref="ProductType"/> business object owned by the module behind the <see cref="IProductManagement"/> facade.
/// </summary>
/// <remarks>
/// Creates a new <see cref="ProductTypeReference"/> in <see cref="ReferenceState.Initialized"/>.
/// </remarks>
[DataContract]
public class ProductTypeReference(string productIdentity) : Reference
{
    /// <summary>
    /// String representation of <see cref="ProductType.Identity"/>.
    /// </summary>
    [DataMember]
    public string ProductIdentity { get; protected set; } = productIdentity ?? throw new ArgumentNullException(nameof(productIdentity));

    /// <summary>
    /// Mapped name of the product type from the underlying business object <see cref="ProductType.Name"/> when active.
    /// This information is reset when the reference is not actively maintained.
    /// </summary>
    public string? Name { get; protected set; }

    /// <summary>
    /// Creates a new instance of a <see cref="ProductInstance"/> for the referenced
    /// <see cref="ProductType"/> when this reference is <see cref="ReferenceState.Active"/>.
    /// Returns <c>null</c> otherwise.
    /// </summary>
    public virtual ProductInstance? CreateInstance() => null;

    /// <inheritdoc />
    public override bool Matches(Reference? other) => other is ProductTypeReference product
        && ProductIdentity == product.ProductIdentity;

    /// <inheritdoc />
    public override string ToString() => $"Product '{ProductIdentity}'";
}