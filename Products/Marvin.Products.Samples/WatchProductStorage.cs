using Marvin.Container;
using Marvin.Model;
using Marvin.Products.Management;

namespace Marvin.Products.Samples
{
    [Plugin(LifeCycle.Singleton, typeof(IProductStorage))]
    public class WatchProductStorage : ProductStorageBase
    {
        public override IUnitOfWorkFactory Factory { get; set; }

        protected override IProductTypeStrategy[] BuildMap()
        {
            return new IProductTypeStrategy[]
            {
                new WatchStrategy(),
                new WatchfaceStrategy(), 
                new DefaultProductStrategy<NeedleProduct>(true, false)
            };
        } 
    }
}
