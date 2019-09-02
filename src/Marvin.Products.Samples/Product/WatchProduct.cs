using System.Collections.Generic;
using System.ComponentModel;
using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    [DisplayName("Uhr")]
    public class WatchProduct : Product
    {
        public override string Type => nameof(WatchProduct);

        // Watch attributes
        [DisplayName("Preis")]
        [Description("Preis der Uhr")]
        public double Price { get; set; }

        [DisplayName("Gewicht")]
        [Description("Rohgewicht der Uhr")]
        public double Weight { get; set; }

        // References to product
        [DisplayName("Ziffernblatt")]
        public ProductPartLink<WatchfaceProduct> Watchface { get; set; }

        [DisplayName("Uhrzeiger")]
        public List<NeedlePartLink> Needles { get; set; } = new List<NeedlePartLink>();

        protected override Article Instantiate()
        {
            return new WatchArticle
            {
                Watchface = (WatchfaceArticle)Watchface.Instantiate(),
                Neddles = Needles.Instantiate<NeedleArticle>()
            };
        }
    }
}
