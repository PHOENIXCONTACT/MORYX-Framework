// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Products.TestProducts;

/// <summary>
/// To test complex data in text columns
/// </summary>
public class ComplexData
{
    // ToDo: Property name "Name" is reserved and is covered by Parent class property so it can not be used here

    public string Content { get; set; }

    public string PropertyName { get; set; }

    public int Number { get; set; }

    public float Weight { get; set; }
}

