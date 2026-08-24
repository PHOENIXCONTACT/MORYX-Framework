// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples;

/// <summary>
/// Test product type for Vector3, Quaternion, Vector4 and Plane serialization.
/// </summary>
public class VectorProductType : ProductType
{
    public Vector3 Position { get; set; }

    public Quaternion Orientation { get; set; }

    public Vector4 Dimensions { get; set; }

    public Plane Surface { get; set; }

    public Vector3? OptionalPosition { get; set; }

    protected override ProductInstance Instantiate()
    {
        throw new NotImplementedException();
    }
}
