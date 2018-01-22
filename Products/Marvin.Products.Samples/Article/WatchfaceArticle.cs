using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class WatchfaceArticle : Article<WatchfaceProduct>
    {
        public override string Type => nameof(WatchfaceArticle);
    }
}