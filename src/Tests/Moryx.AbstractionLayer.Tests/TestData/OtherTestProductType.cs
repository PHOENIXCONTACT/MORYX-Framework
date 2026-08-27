// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.AbstractionLayer.Tests.TestData;

public class OtherTestProductType : ProductType
{
    protected override ProductInstance Instantiate() => new OtherTestProductInstance();
}
