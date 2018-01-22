using Marvin.AbstractionLayer;
using Marvin.Model;
using Marvin.Products.Management;
using Marvin.Products.Model;

namespace Marvin.Products.Samples
{
    public class WatchfaceStrategy : DefaultProductStrategy<WatchfaceProduct>
    {
        public WatchfaceStrategy() : base(false, true)
        {
        }

        public override IProduct LoadProduct(IUnitOfWork uow, ProductEntity entity)
        {
            var watchface = (WatchfaceProduct) base.LoadProduct(uow, entity);
            watchface.Numbers = new[] {3, 6, 9, 12};
            return watchface;
        }

        /// <summary>
        /// Save product to database
        /// </summary>
        public override ProductEntity SaveProduct(IUnitOfWork uow, IProduct product)
        {
            var watchfaceEntity = GetProductEntity(uow, product);
            var properties = CreateVersion(uow.GetRepository<IProductPropertiesRepository>(), product, watchfaceEntity);

            var watchFace = (WatchfaceProduct)product;

            // save watchNumbers
            //properties.Numbers = watchFace.Numbers;

            return watchfaceEntity;
        }
    }
}