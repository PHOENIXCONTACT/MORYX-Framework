using Marvin.Container;
using Marvin.Model;
using Marvin.Products.Management;
using Marvin.Products.Samples.Model;

namespace Marvin.Products.Samples
{
    [Plugin(LifeCycle.Singleton, typeof(IProductStorage))]
    public class WatchProductStorage : ProductStorageBase
    {
        [UseChild(WatchProductsConstants.Namespace)]
        public override IUnitOfWorkFactory Factory { get; set; }

        protected override IProductTypeStrategy[] BuildMap()
        {
            return new IProductTypeStrategy[]
            {
                new WatchStrategy(),
                new WatchFaceStrategy(), 
                new DefaultProductStrategy<NeedleProduct>(true, false)
            };
        } 
    }
}
