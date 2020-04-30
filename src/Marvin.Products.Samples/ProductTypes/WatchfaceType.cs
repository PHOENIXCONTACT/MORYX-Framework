// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Marvin.AbstractionLayer.Products;

namespace Marvin.Products.Samples
{
    [DisplayName("Watchface")]
    public class WatchfaceType : ProductType
    {
        [Description("Numbers on the watchface")]
        public int[] Numbers { get; set; }

        [Description("Digital or Analog")]
        public bool IsDigital { get; set; }

        protected override ProductInstance Instantiate()
        {
            return new WatchfaceInstance();
        }
    }
}
