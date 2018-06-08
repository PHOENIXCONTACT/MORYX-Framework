using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class WatchFaceProduct : Product
    {
        public override string Type => nameof(WatchFaceProduct);

        public int[] Numbers { get; set; }

        public bool IsDigital { get; set; }

        protected override Article Instantiate()
        {
            return new WatchFaceArticle();
        }
    }
}