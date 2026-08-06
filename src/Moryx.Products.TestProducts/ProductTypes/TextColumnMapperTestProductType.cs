// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer.Products;
namespace Moryx.Products.TestProducts;

[DisplayName("TextColumnMapper Test Type")]
public class TextColumnMapperTestProductType : ProductType
{
    public int Integer1 { get; set; }

    public int Integer2 { get; set; }

    public double Float1 { get; set; }

    public string MyText1 { get; set; }

    public ComplexData ComplexData1 { get; set; }
    
    public TestProdData ProdDataAdded { get; set; }

    protected override ProductInstance Instantiate()
    {
        return new TextColumnMapperTestProductTypeInstance();
    }
}
