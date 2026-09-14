// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;

namespace Moryx.Products.TestProducts;

/// <summary>
/// To test complex data in text columns
/// </summary>
public class ComplexData
{
    // ToDo: Property name "Name" is reserved and is covered by Parent class property so it can not be used here
    [Description("inner Name")]
    public string Name { get; set; }

    [Description("Content")]
    public string Content { get; set; }

    [Description("Property name")]
    public string PropertyName { get; set; }

    [Description("Just a number")]
    public int Number { get; set; }

    [Description("Weight of product")]
    public float Weight { get; set; }
}

