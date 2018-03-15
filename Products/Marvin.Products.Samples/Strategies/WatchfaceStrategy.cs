using Marvin.AbstractionLayer;
using Marvin.Model;
using Marvin.Products.Management;
using Marvin.Products.Model;

namespace Marvin.Products.Samples
{
    public class WatchFaceStrategy : DefaultProductStrategy<WatchFaceProduct>
    {
        public WatchFaceStrategy() : base(false, true)
        {
        }

        public override IProduct LoadProduct(IUnitOfWork uow, ProductEntity entity)
        {
            var watchface = (WatchFaceProduct) base.LoadProduct(uow, entity);
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

            var watchFace = (WatchFaceProduct)product;

            // save watchNumbers
            //properties.Numbers = watchFace.Numbers;

            return watchfaceEntity;
        }
    }
}