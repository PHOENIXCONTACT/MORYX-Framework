// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples
{
    public class WatchFaceInstance : ProductInstance<WatchFaceTypeBase>, IIdentifiableObject
    {
        public Guid Identifier { get; set; }

        public IIdentity Identity { get; set; }

        public WatchFaceInstance()
        {
            Identifier = Guid.NewGuid();
        }

    }
}
