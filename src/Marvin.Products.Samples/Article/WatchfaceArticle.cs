using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class WatchFaceArticle : Article<WatchfaceProduct>
    {
        public override string Type => nameof(WatchFaceArticle);
    }
}