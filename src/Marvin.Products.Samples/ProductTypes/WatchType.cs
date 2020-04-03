// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Products;

namespace Marvin.Products.Samples
{
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
        [DisplayName("Wach face")]
        public ProductPartLink<WatchfaceType> Watchface { get; set; }

        [DisplayName("Watch needle")]
        public List<NeedlePartLink> Needles { get; set; } = new List<NeedlePartLink>();

        protected override ProductInstance Instantiate()
        {
            return new WatchInstance
            {
                Watchface = (WatchfaceInstance)Watchface.Instantiate(),
                Needles = Needles.Instantiate<NeedleInstance>()
            };
        }
    }
}
