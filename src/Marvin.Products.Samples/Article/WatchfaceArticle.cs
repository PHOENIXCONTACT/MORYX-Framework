using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class WatchFaceArticle : Article<WatchFaceProduct>
    {
        public override string Type => nameof(WatchFaceArticle);
    }
}