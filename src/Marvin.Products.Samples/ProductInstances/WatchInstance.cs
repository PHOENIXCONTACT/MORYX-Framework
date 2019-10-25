using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class WatchInstance : ProductInstance<WatchType>
    {
        public override string Type => nameof(WatchInstance);

        public bool TimeSet { get; set; }

        public DateTime DeliveryDate { get; set; }

        public WatchfaceInstance Watchface { get; set; }

        public ICollection<NeedleInstance> Needles { get; set; }
    }
}