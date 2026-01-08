// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Products;

/// <summary>
/// Base interface for the link
/// </summary>
public interface IProductPartLink : IPersistentObject
{
    /// <summary>
    /// Generic access to the product of this part link
    /// </summary>
    ProductType Product { get; set; }

    /// <summary>
    /// Create single product instance for this part
    /// </summary>
    ProductInstance Instantiate();
}

/// <summary>
/// API for part wrapper
/// </summary>
/// <typeparam name="TProduct"></typeparam>
public interface IProductPartLink<TProduct> : IProductPartLink
    where TProduct : ProductType
{
    /// <summary>
    /// Typed product of this part
    /// </summary>
    new TProduct Product { get; set; }
}

/// <summary>
/// Extension to instantiate instance collection from product type parts collection
/// </summary>
public static class PartLinkExtension
{
    /// <summary>
    /// Instantiate product instance collection
    /// </summary>
    public static List<TInstance> Instantiate<TInstance>(this IEnumerable<ProductPartLink> parts)
        where TInstance : ProductInstance
    {
        return parts.Select(p => (TInstance)p.Instantiate()).ToList();
    }
}