// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples;

/// <summary>
/// Product type used for integration tests of property-to-column mappings.
/// </summary>
[DisplayName("Composite Test Type")]
public class CompositeProductType : ProductType
{
    /// <summary>
    /// Complex property with a dedicated column
    /// </summary>
    public ComplexData ComplexData1 { get; set; }

    /// <summary>
    /// Complex property without a column, stored within the JsonColumn
    /// </summary>
    public ComplexData ComplexData2 { get; set; }

    protected override ProductInstance Instantiate()
    {
        throw new NotImplementedException();
    }
}
