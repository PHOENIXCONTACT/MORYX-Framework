// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples;

[DisplayName("Watch")]
public class WatchType : ProductType
{
    // Watch attributes
    [Description("Price of the watch")]
    public double Price { get; set; }

    [DisplayName("Raw weight")]
    [Description("Weight without packaging")]
    public double Weight { get; set; }

    // References to product
    [DisplayName("Watch face")]
    public ProductPartLink<WatchFaceTypeBase> WatchFace { get; set; }

    [DisplayName("Watch needle")]
    public List<NeedlePartLink> Needles { get; set; } = [];

    protected override ProductInstance Instantiate()
    {
        return new WatchInstance
        {
            WatchFace = (WatchFaceInstance)WatchFace.Instantiate(),
            Needles = Needles.Instantiate<NeedleInstance>()
        };
    }
}