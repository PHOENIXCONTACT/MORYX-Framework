// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples
{
    public class WatchInstance : ProductInstance<WatchType>, IIdentifiableObject
    {
        public IIdentity Identity { get; set; }

        public bool TimeSet { get; set; }

        public DateTime DeliveryDate { get; set; }

        public WatchFaceInstance WatchFace { get; set; }

        public ICollection<NeedleInstance> Needles { get; set; }
    }
}
