// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.TestProducts;

/// <summary>
/// Test product type for Vector3 and Quaternion serialization.
/// </summary>
public class VectorProductType : ProductType
{
    public Vector3 Position { get; set; }

    public Quaternion Orientation { get; set; }

    protected override ProductInstance Instantiate()
    {
        throw new NotImplementedException();
    }
}
