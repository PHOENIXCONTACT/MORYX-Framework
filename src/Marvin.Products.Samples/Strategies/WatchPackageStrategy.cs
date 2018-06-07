using Marvin.Products.Management;

namespace Marvin.Products.Samples
{
    internal class WatchPackageStrategy : DefaultProductStrategy<WatchPackageProduct>
    {
        public WatchPackageStrategy() : base(false, ParentLoadBehaviour.Ignore)
        {
            Parts = new ILinkStrategy[]
            {
                new DefaultLinkStrategy<WatchProduct>(nameof(WatchPackageProduct.PossibleWatches))
            };
        }
    }
}