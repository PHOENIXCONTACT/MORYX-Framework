using System;
using Marvin.AbstractionLayer;
using Marvin.Model;
using Marvin.Products.Management;
using Marvin.Products.Model;
using Marvin.Products.Samples.Model;

namespace Marvin.Products.Samples
{
    public class WatchStrategy : ProductStrategyBase, IProductTypeStrategy
    {
        public WatchStrategy()
        {
            Parts = new ILinkStrategy[]
            {
                new DefaultLinkStrategy<WatchFaceProduct>(nameof(WatchProduct.Watchface)),
                new NeedleLinkStrategy()
            };
        }

        public string TargetType => nameof(WatchProduct);

        public bool IncludeParent => false;

        public ILinkStrategy[] Parts { get; }

        public IProduct LoadProduct(IUnitOfWork uow, ProductEntity entity)
        {
            // Load extended repo and entity here

            var properties = entity.CurrentVersion as AnalogWatchProductPropertiesEntity;

            // Transform watch
            var watch = new WatchProduct
            {
                Weight = 123.1,
                Price = 1299.99
            };
            CopyToProduct(entity, watch);

            return watch;
        }

        public ProductEntity SaveProduct(IUnitOfWork uow, IProduct product)
        {
            var propRepo = uow.GetRepository<IProductPropertiesRepository>();

            var watch = (WatchProduct)product;

            var watchEntity = GetProductEntity(uow, product);
            CreateVersion(propRepo, product, watchEntity);

            return watchEntity;
        }

        public bool SkipArticles => false;

        public PartSourceStrategy ArticleCreation => PartSourceStrategy.FromPartlink;

        public void LoadArticle(IUnitOfWork uow, ArticleEntity entity, Article article)
        {
            var watch = (WatchArticle)article;

            CopyToArticle(entity, article);

            // Restore TimeSet flag
            watch.TimeSet = (entity.State >> 8) >= 1;

            // Restore date
            var binaryDate = long.Parse(entity.ExtensionData);
            watch.ProductionDate = DateTime.FromBinary(binaryDate);
        }

        public ArticleEntity SaveArticle(IUnitOfWork uow, Article article)
        {
            var watch = (WatchArticle)article;

            var entity = GetArticleEntity(uow, article);
            CopyToArticleEntity(article, entity, true);

            // Include TimeSet-flag in state
            if (watch.TimeSet)
                entity.State |= (1 << 8);

            // Save date as binary
            var binaryDate = watch.DeliveryDate.ToBinary();
            entity.ExtensionData = binaryDate.ToString("X");

            return entity;
        }

        private class NeedleLinkStrategy : DefaultLinkStrategy<NeedleProduct>
        {
            protected internal NeedleLinkStrategy() : base(nameof(WatchProduct.Needles))
            {
            }

            //public override PartSourceStrategy PartCreation => PartSourceStrategy.FromEntities;

            public override IProductPartLink Load(IUnitOfWork uow, PartLink linkEntity)
            {
                return new NeedlePartLink(linkEntity.Id) { Role = NeedleRole.Minutes };
            }

            // ReSharper disable once RedundantOverridenMember <-- For demonstration
            public override PartLink Create(IUnitOfWork uow, IProductPartLink link)
            {
                return base.Create(uow, link);
            }
        }
    }
}