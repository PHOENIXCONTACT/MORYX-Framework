// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples
{
    public class WatchfaceInstance : ProductInstance<WatchfaceType>
    {
        public Guid Identifier { get; set; }

        public WatchfaceInstance()
        {
            Identifier = Guid.NewGuid();
        }
    }
}
