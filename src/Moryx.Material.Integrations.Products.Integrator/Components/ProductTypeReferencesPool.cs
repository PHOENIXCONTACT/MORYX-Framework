// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products.Integrator.Components;

/// <summary>
/// Lifecycle pool of <see cref="InternalProductTypeReference"/> instances that are
/// currently referenced by at least one product-linked container. References are added on
/// demand (through <see cref="AcquireAsync"/> / <see cref="ResolveAsync"/>) and removed via
/// <see cref="Release"/> once no container uses them anymore.
/// </summary>
[Component(LifeCycle.Singleton, typeof(IProductTypeReferencesPool))]
internal class ProductTypeReferencesPool : IProductTypeReferencesPool
{
    private readonly ConcurrentDictionary<string, PooledEntry> _references = new();
    private readonly Lock _resolveLock = new();

    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IProductManagement ProductManagement { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    #region Lifecycle
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // We do not preload product types. The pool grows on demand when containers
        // declare their linked product references.
        ProductManagement.TypeChanged += OnTypeChanged;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        ProductManagement.TypeChanged -= OnTypeChanged;

        foreach (var entry in _references.Values)
        {
            Deactivate(entry.Reference);
        }
        _references.Clear();

        return Task.CompletedTask;
    }

    private void OnTypeChanged(object? sender, ProductType productType)
    {
        var key = productType.Identity?.ToString();
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        // Only synchronize references that are already tracked by the pool. We do not
        // introduce new entries for product types no container is linked to.
        if (_references.TryGetValue(key, out var entry))
        {
            entry.Reference.Name = productType.Name;
            entry.Reference.Status = productType.State;
            entry.Reference.ProductType = productType;
            entry.Reference.State = ReferenceState.Active;
        }
    }

    private static void Deactivate(InternalProductTypeReference reference)
    {
        reference.State = ReferenceState.Inactive;
        reference.Status = null;
        reference.ProductType = null;
    }
    #endregion

    #region IProductTypeReferencesPool

    public InternalProductTypeReference? Get(ProductTypeReference? reference)
    {
        if (reference is null || string.IsNullOrEmpty(reference.ProductIdentity))
        {
            return null;
        }

        return _references.TryGetValue(reference.ProductIdentity, out var entry) ? entry.Reference : null;
    }

    public async Task<InternalProductTypeReference> ResolveAsync(string productIdentity, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(productIdentity))
        {
            throw new ArgumentException("Product identity must not be empty.", nameof(productIdentity));
        }

        // Check existing references first. We do not add unpooled references to the pool
        // here: pool membership is only established when a container actually acquires the
        // reference (see <see cref="AcquireAsync"/>).
        if (_references.TryGetValue(productIdentity, out var entry))
        {
            return entry.Reference;
        }

        // Fall back to the facade if we have not seen this identity before.
        var product = await ProductManagement.LoadProductFor(productIdentity);
        return product is not null
            ? product.ToReference()
            : new InternalProductTypeReference(productIdentity) { State = ReferenceState.Unavailable };
    }

    public async Task<InternalProductTypeReference> AcquireAsync(ProductTypeReference reference, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reference);
        if (string.IsNullOrEmpty(reference.ProductIdentity))
        {
            throw new ArgumentException("Reference must carry a product identity.", nameof(reference));
        }

        // If a matching entry is already tracked, just bump its usage counter.
        if (_references.TryGetValue(reference.ProductIdentity, out var existing))
        {
            existing.Acquire();
            return existing.Reference;
        }

        // If the caller provides an already-resolved internal reference (e.g. from an earlier
        // <see cref="ResolveAsync"/> call), reuse it as the pool entry to keep instance identity.
        if (reference is InternalProductTypeReference preResolved)
        {
            return AddOrUpdate(preResolved, incrementUsage: true);
        }

        // Otherwise resolve via the facade and add a new entry.
        var product = await ProductManagement.LoadProductFor(reference.ProductIdentity);
        return AddOrUpdate(reference.ProductIdentity, product, incrementUsage: true);
    }

    public void Release(ProductTypeReference? reference)
    {
        if (reference is null || string.IsNullOrEmpty(reference.ProductIdentity))
        {
            return;
        }

        if (!_references.TryGetValue(reference.ProductIdentity, out var entry))
        {
            return;
        }

        if (entry.Release())
        {
            // Last user gone: remove from pool. We serialize with resolution to avoid
            // a race between removal and a concurrent acquire.
            lock (_resolveLock)
            {
                if (entry.UsageCount == 0 && _references.TryRemove(reference.ProductIdentity, out _))
                {
                    Deactivate(entry.Reference);
                }
            }
        }
    }

    public IReadOnlyList<ProductTypeReference> GetAll() => [.. _references.Values.Select(e => (ProductTypeReference)e.Reference)];

    #endregion

    private InternalProductTypeReference AddOrUpdate(string productIdentity, ProductType? product, bool incrementUsage)
    {
        lock (_resolveLock)
        {
            var entry = _references.GetOrAdd(productIdentity, key =>
            {
                var reference = product is not null
                    ? product.ToReference()
                    : new InternalProductTypeReference(key) { State = ReferenceState.Unavailable };
                return new PooledEntry(reference);
            });

            // Update mapped properties if we now know the product.
            if (product is not null)
            {
                entry.Reference.Name = product.Name;
                entry.Reference.Status = product.State;
                entry.Reference.ProductType = product;
                entry.Reference.State = ReferenceState.Active;
            }

            if (incrementUsage)
            {
                entry.Acquire();
            }

            return entry.Reference;
        }
    }

    private InternalProductTypeReference AddOrUpdate(InternalProductTypeReference reference, bool incrementUsage)
    {
        lock (_resolveLock)
        {
            var entry = _references.GetOrAdd(reference.ProductIdentity, _ => new PooledEntry(reference));

            if (incrementUsage)
            {
                entry.Acquire();
            }

            return entry.Reference;
        }
    }

    /// <summary>
    /// Small holder tracking how many containers currently use a pooled reference.
    /// </summary>
    private sealed class PooledEntry(InternalProductTypeReference reference)
    {
        private int _usageCount;

        public InternalProductTypeReference Reference { get; } = reference;

        public int UsageCount => Volatile.Read(ref _usageCount);

        public void Acquire() => Interlocked.Increment(ref _usageCount);

        /// <summary>
        /// Decrements the usage counter and returns <c>true</c> when it drops to zero.
        /// </summary>
        public bool Release() => Interlocked.Decrement(ref _usageCount) <= 0;
    }
}