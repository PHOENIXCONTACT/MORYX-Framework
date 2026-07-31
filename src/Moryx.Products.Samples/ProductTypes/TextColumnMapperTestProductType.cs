// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer.Products;
using Moryx.Products.Samples.ProductTypes;

namespace Moryx.Products.Samples;

[DisplayName("TextColumnMapper Test Type")]
public class TextColumnMapperTestProductType : ProductType
{
    public int Integer1 { get; set; }

    public int Integer2 { get; set; }

    public double Float1 { get; set; }

    public string MeinText1 { get; set; }

    public ComplexData ComplexData1 { get; set; }
    
    public TestProdData ProdDataAdded { get; set; }

    protected override ProductInstance Instantiate()
    {
        return new TextColumnMapperTestProductTypeInstance();
    }
}
