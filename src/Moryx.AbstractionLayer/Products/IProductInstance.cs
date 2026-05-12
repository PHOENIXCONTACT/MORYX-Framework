// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Products;

/// <summary>
/// Interface for product instances
/// </summary>
public interface IProductInstance
{
    /// <summary>
    /// Id of the instance
    /// </summary>
    long Id { get; }

    /// <summary>
    /// The product type of this instance
    /// </summary>
    ProductType Type { get; }

    /// <summary>
    /// Part link that created this <see cref="IProductInstance"/>. This is <value>null</value> for root instances
    /// </summary>
    ProductPartLink PartLink { get; }

    /// <summary>
    /// The current state of the instance
    /// </summary>
    ProductInstanceState State { get; set; }
}

/// <summary>
/// Extended product instance
/// </summary>
public interface IProductInstanceExtended : IProductInstance
{
    /// <summary>
    /// The parent product instance, null if it has no parent.
    /// </summary>
    ProductInstance Parent { get; }
}
