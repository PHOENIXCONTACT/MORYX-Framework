using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class WatchArticle : Article<WatchProduct>
    {
        public override string Type => nameof(WatchArticle);

        public bool TimeSet { get; set; }

        public DateTime DeliveryDate { get; set; }

        public WatchFaceArticle WatchFace
        {
            get { return Single<WatchFaceArticle>().Part; }
            set { Single<WatchFaceArticle>().Part = value; }
        }

        public ICollection<NeedleArticle> Neddles
        {
            get { return Multiple<NeedleArticle>(); }
            set { Multiple(value); }
        }
    }
}