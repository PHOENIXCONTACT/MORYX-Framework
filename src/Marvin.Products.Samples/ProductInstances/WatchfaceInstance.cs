using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class WatchfaceInstance : ProductInstance<WatchfaceType>
    {
        public override string Type => nameof(WatchfaceInstance);
    }
}