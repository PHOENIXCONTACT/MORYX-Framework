// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.TestProducts;

/// <summary>
/// Product type used for integration tests of property-to-column mappings.
/// </summary>
[DisplayName("Composite Test Type")]
public class CompositeProductType : ProductType
{
    public ComplexData ComplexData1 { get; set; }

    protected override ProductInstance Instantiate()
    {
        throw new NotImplementedException();
    }
}
