// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples;

/// <summary>
/// Test product type for DateOnly, TimeOnly and DateTimeOffset serialization.
/// </summary>
public class DatedProductType : ProductType
{
    public DateOnly ValidFrom { get; set; }

    public TimeOnly ProductionTime { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    protected override ProductInstance Instantiate()
    {
        throw new NotImplementedException();
    }
}
