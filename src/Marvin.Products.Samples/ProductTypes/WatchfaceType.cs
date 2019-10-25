using System.ComponentModel;
using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    [DisplayName("Watchface")]
    public class WatchfaceType : ProductType
    {
        public override string Type => nameof(WatchfaceType);

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