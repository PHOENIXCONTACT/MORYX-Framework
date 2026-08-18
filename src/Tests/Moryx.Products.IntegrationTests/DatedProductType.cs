// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.IntegrationTests;

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
