// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Material.Linking.Integration;
using Moryx.Modules;

namespace Moryx.Material.Integrations.Products.Integrator.Components;

/// <summary>
/// Lifecycle pool for managed <see cref="InternalProductTypeReference"/> instances that are
/// currently referenced by at least one <see cref="IProductLinkedMaterialContainer"/>.
/// </summary>
/// <remarks>
/// The pool is populated on demand: entries are added when a container declares a link and
/// removed once the last container referencing them is deregistered. References are kept in
/// sync with the <see cref="IProductManagement"/> facade for as long as they live.
/// <see cref="IReferencePool{TReference}.AcquireAsync"/> increments the usage count of a reference,
/// <see cref="IReferencePool{TReference}.Release"/> decrements it and removes the reference once
/// no container uses it anymore.
/// </remarks>
internal interface IProductTypeReferencesPool : IAsyncPlugin, IReferencePool<ProductTypeReference>
{
    /// <summary>
    /// Gets the managed reference matching <paramref name="reference"/>, if one is currently
    /// held by the pool.
    /// </summary>
    InternalProductTypeReference? Get(ProductTypeReference? reference);

    /// <summary>
    /// Resolves the managed reference for the given product identity.
    /// </summary>
    /// <remarks>
    /// The pool first checks for an existing entry with the same identity. Only if no entry
    /// exists yet, the <see cref="IProductManagement"/> facade is queried to determine
    /// whether the product exists. The resulting reference is added to the pool and returned.
    /// </remarks>
    Task<InternalProductTypeReference> ResolveAsync(string productIdentity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all currently managed references.
    /// </summary>
    IReadOnlyList<ProductTypeReference> GetAll();
}