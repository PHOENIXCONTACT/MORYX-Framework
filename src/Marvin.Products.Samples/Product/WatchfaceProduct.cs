using System.ComponentModel;
using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    [DisplayName("Ziffernblatt")]
    public class WatchfaceProduct : Product
    {
        public override string Type => nameof(WatchfaceProduct);

        [DisplayName("Ziffern")]
        [Description("Ziffern auf dem Blatt")]
        public int[] Numbers { get; set; }

        [DisplayName("Ist Digital")]
        [Description("Digital oder Analog")]
        public bool IsDigital { get; set; }

        protected override Article Instantiate()
        {
            return new WatchfaceArticle();
        }
    }
}