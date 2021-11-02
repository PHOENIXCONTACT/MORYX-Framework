// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples
{
    public class WatchfaceInstance : ProductInstance<WatchfaceTypeBase>, IIdentifiableObject
    {
        public Guid Identifier { get; set; }

        public IIdentity Identity { get; set; }

        public WatchFaceInstance()
        {
            Identifier = Guid.NewGuid();
        }

    }
}
