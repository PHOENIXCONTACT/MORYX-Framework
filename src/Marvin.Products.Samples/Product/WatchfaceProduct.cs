using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class WatchfaceProduct : Product
    {
        public override string Type => nameof(WatchfaceProduct);

        public int[] Numbers { get; set; }

        public bool IsDigital { get; set; }

        protected override Article Instantiate()
        {
            return new WatchFaceArticle();
        }
    }
}