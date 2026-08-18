// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.TestProducts;

/// <summary>
/// Product type used for integration tests of JSON column mappings.
/// </summary>
[DisplayName("Generic Json Test type")]
public class GenericJsonTestProductType : ProductType
{

    protected override ProductInstance Instantiate()
    {
        return new GenericJsonTestProductTypeInstance();
    }

    [Description("Sample Int 1")]
    // [DefaultValue(1)]
    public int Integer1 { get; set; }

    [Description("Sample Int 2")]
    // [DefaultValue(2)]
    public int Integer2 { get; set; }

    [Description("Sample Int 3")]
    // [DefaultValue(3)]
    public int Integer3 { get; set; }

    [Description("Sample Int 4")]
    // [DefaultValue(4)]
    public int Integer4 { get; set; }

    [Description("Sample Int 5")]
    // [DefaultValue(5)]
    public int Integer5 { get; set; }

    [Description("Sample Int 6")]
    // [DefaultValue(6)]
    public int Integer6 { get; set; }

    [Description("Sample Int 7")]
    // [DefaultValue(7)]
    public int Integer7 { get; set; }

    [Description("Sample Int 8")]
    // [DefaultValue(8)]
    public int Integer8 { get; set; }

    [Description("Sample Int 9")]
    // [DefaultValue(9)]
    public int Integer9 { get; set; }

    [Description("Sample Int 10")]
    // [DefaultValue(10)]
    public int Integer10 { get; set; }

    [Description("Sample float 1")]
    // [DefaultValue(1.1)]
    public double Float1 { get; set; }

    [Description("Sample float 2")]
    // [DefaultValue(2.1)]
    public float Float2 { get; set; }

    [Description("Sample float 3")]
    // [DefaultValue(3.1)]
    public float Float3 { get; set; }

    [Description("Sample float 4")]
    // [DefaultValue(4.1)]
    public float Float4 { get; set; }

    [Description("Sample float 5")]
    // [DefaultValue(5.1)]
    public float Float5 { get; set; }

    [Description("Sample float 6")]
    // [DefaultValue(6.1)]
    public float Float6 { get; set; }

    [Description("Sample float 7")]
    // [DefaultValue(7.1)]
    public float Float7 { get; set; }

    [Description("Sample float 8")]
    // [DefaultValue(8.1)]
    public float Float8 { get; set; }

    [Description("Sample float 9")]
    // [DefaultValue(9.1)]
    public double Float9 { get; set; }

    [Description("Sample float 10")]
    // [DefaultValue(10.1)]
    public float Float10 { get; set; }

    //[Description("Sample Text 1")]
    //// [DefaultValue("Text 1")]
    //public string Text1 { get; set; }

    [Description("Sample Text 2")]
    // [DefaultValue("Text 2")]
    public string Text2 { get; set; }

    [Description("Sample Text 3")]
    // [DefaultValue("Text 3")]
    public string Text3 { get; set; }

    [Description("Sample Text 4")]
    // [DefaultValue("Text 4")]
    public string Text4 { get; set; }

    [Description("Sample Text 5")]
    // [DefaultValue("Text 5")]
    public string Text5 { get; set; }

    [Description("Sample Text 6")]
    // [DefaultValue("Text 6")]
    public string Text6 { get; set; }

    [Description("Sample Text 7")]
    // [DefaultValue("Text 7")]
    public string Text7 { get; set; }

    [Description("Sample Text 8")]
    // [DefaultValue("Text 8")]
    public string Text8 { get; set; }

    [Description("Sample Text 9")]
    // [DefaultValue("Text 9")]
    public string Text9 { get; set; }

    [Description("Sample Text 10")]
    // [DefaultValue("Text 10")]
    public string Text10 { get; set; }

    [Description("Complex data for a text column")]
    public ComplexData ComplexData1 { get; set; }

}
