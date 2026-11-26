// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Moryx.AbstractionLayer.Products;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Products.Integrator.Components;

internal class InternalProductTypeReference : ProductTypeReference
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new required string ProductIdentity
    {
        get;
        set => base.ProductIdentity = field = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new string? Name
    {
        get;
        set => base.Name = field = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new ReferenceState State
    {
        get;
        set => base.State = field = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ProductType? ProductType { get; set; }

    public override ProductInstance? CreateInstance()
    {
        // Only create an instance while we actively hold a strong reference to the ProductType.
        if (this.IsActive())
        {
            return ProductType?.CreateInstance();
        }

        return null;
    }

    [SetsRequiredMembers]
    internal InternalProductTypeReference(string productIdentity) : base(productIdentity)
    {
        ProductIdentity = productIdentity;
    }
}

internal static class ReferenceExtensions
{
    extension(ProductType productType)
    {
        public InternalProductTypeReference ToReference() => new(productType.Identity?.ToString() ?? string.Empty)
        {
            Name = productType.Name,
            ProductType = productType,
            State = ReferenceState.Active
        };
    }

    // ToDo: Verify if extensions make sense
    extension(IProductManagement productManagement)
    {
        public async Task<ProductType?> LoadProductFor(ProductTypeReference? reference)
        {
            if (reference is null || string.IsNullOrEmpty(reference.ProductIdentity))
            {
                return null;
            }
            return await productManagement.LoadProductFor(reference.ProductIdentity);
        }

        public async Task<ProductType?> LoadProductFor(string productIdentity)
        {
            if (!ProductIdentity.TryParse(productIdentity, out var identity))
            {
                return null;
            }

            try
            {
                return await productManagement.LoadTypeAsync(identity);
            }
            catch (ProductNotFoundException)
            {
                return null;
            }
        }
    }
}
