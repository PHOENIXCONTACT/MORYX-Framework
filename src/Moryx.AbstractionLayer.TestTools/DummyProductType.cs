// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.AbstractionLayer.TestTools;

/// <summary>
/// Dummy implementation of a <see cref="ProductType"/>
/// </summary>
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
public class DummyProductType : ProductType
{
    /// <inheritdoc />
    protected override ProductInstance Instantiate()
    {
        return new DummyProductInstance();
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        if (obj is not DummyProductType toCompareWith)
        {
            return false;
        }

        return toCompareWith.Id == Id && toCompareWith.Name == Name && toCompareWith.State == State
               && ((toCompareWith.Identity is null && Identity is null) || toCompareWith.Identity.Equals(Identity));
    }
}

/// <summary>
/// Dummy implementation of a <see cref="ProductType"/> with Product Parts
/// </summary>
public class DummyProductTypeWithParts : DummyProductType
{
    /// <inheritdoc />
    protected override ProductInstance Instantiate()
    {
        return new DummyProductInstance();
    }

    /// <summary>
    /// Dummy ProductPartLink
    /// </summary>
    public DummyProductPartLink ProductPartLink { get; set; }

    /// <summary>
    /// Dummy ProductPartLink enumerable
    /// </summary>
    public IEnumerable<DummyProductPartLink> ProductPartLinkEnumerable { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        if (obj is not DummyProductTypeWithParts toCompareWith)
        {
            return false;
        }

        return base.Equals(toCompareWith)
            && ((toCompareWith.ProductPartLink is null && ProductPartLink is null)
            || toCompareWith.ProductPartLink.Equals(ProductPartLink))
            && ((toCompareWith.ProductPartLinkEnumerable is null && ProductPartLinkEnumerable is null)
            || Enumerable.SequenceEqual(toCompareWith.ProductPartLinkEnumerable, ProductPartLinkEnumerable));
    }
}
