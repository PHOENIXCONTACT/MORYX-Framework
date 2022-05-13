// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples
{
    public abstract class WatchfaceTypeBase : ProductType
    {
    }

    [DisplayName("Display Watchface")]
    public class DisplayWatchfaceType : WatchfaceTypeBase
    {
        [Description("Screen resolution in DPI")]
        public int Resolution { get; set; }

        protected override ProductInstance Instantiate()
        {
            return new WatchfaceInstance();
        }
    }

    [DisplayName("Watchface")]
    public class WatchfaceType : WatchfaceTypeBase
    {
        [Description("Numbers on the watchface")]
        public int[] Numbers { get; set; }

        [Description("Digital or Analog")]
        public bool IsDigital { get; set; }

        [DisplayName("Brand")]
        public string Brand { get; set; }

        [DisplayName("Color")]
        public int Color { get; set; }

        public string NumbersString => string.Join(", ", Numbers);

        protected override ProductInstance Instantiate()
        {
            return new WatchfaceInstance();
        }
    }
}
