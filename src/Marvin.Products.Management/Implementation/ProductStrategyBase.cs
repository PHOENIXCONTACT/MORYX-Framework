using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Identity;
using Marvin.Model;
using Marvin.Products.Model;
using Marvin.Tools;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Base class for product strategies
    /// </summary>
    public abstract class ProductStrategyBase
    {
        /// <summary>
        /// Copy database values to the product object
        /// </summary>
        protected static void CopyToProduct(ProductEntity from, Product to)
        {
            to.Id = from.Id;
            to.Identity = new ProductIdentity(from.Identifier, from.Revision);
            to.Name = from.CurrentVersion.Name;
        }

        /// <summary>
        ///  Get or create product entity for the business object
        /// </summary>
        protected static ProductEntity GetProductEntity(IUnitOfWork uow, IProduct product)
        {
            var repo = uow.GetRepository<IProductEntityRepository>();
            var identity = (ProductIdentity)product.Identity;
            ProductEntity productEntity;
            var productEntities = repo.Linq
                .Where(p => p.Identifier == identity.Identifier && p.Revision == identity.Revision)
                .ToList();
            // If entity does not exist or was deleted, create a new one
            if (productEntities.Count == 0 || productEntities.All(p => p.Deleted != null))
            {
                productEntity = repo.Create(identity.Identifier, identity.Revision, product.Type);
                EntityIdListener.Listen(productEntity, product);
            }
            else
            {
                // Set id in case it was imported under existing material and revision
                productEntity = productEntities.First(p => p.Deleted == null);
                product.Id = productEntity.Id;
            }
            return productEntity;
        }

        /// <summary>
        /// Create version instance by using model merge enhanced repo
        /// </summary>
        protected static TVersion CreateVersion<TVersion>(IRepository<TVersion> repo, IProduct product, ProductEntity entity)
            where TVersion : class
        {
            // Create new version
            var version = repo.Create();

            var properties = version as ProductProperties;
            properties.Name = product.Name ?? string.Empty;
            entity.SetCurrentVersion(properties);

            return version;
        }

        /// <summary>
        /// Copy basic article properties to the business object
        /// </summary>
        protected static Article CopyToArticle(ArticleEntity entity, Article baseArticle)
        {
            baseArticle.Id = entity.Id;
            baseArticle.ProductionDate = entity.ProductionDate;
            baseArticle.State = (ArticleState)(entity.State & 0x0F); // Use mask in case other enums were bit-shifted into the state
            if ((entity.State & 0x10) == 1)
                baseArticle.Reworked = true;

            return baseArticle;
        }

        /// <summary>
        ///  Get or create product entity for the business object
        /// </summary>
        protected static ArticleEntity GetArticleEntity(IUnitOfWork uow, Article article)
        {
            return uow.GetEntity<ArticleEntity>(article);
        }

        /// <summary>
        /// Copy all properties to the entity
        /// </summary>
        protected static ArticleEntity CopyToArticleEntity(Article baseArticle, ArticleEntity entity, bool includeIdentity)
        {
            // Copy article properties
            entity.Id = baseArticle.Id;
            entity.State = (int)baseArticle.State;
            if (baseArticle.Reworked)
                entity.State |= (1 << 4);
            entity.ProductId = baseArticle.Product.Id;
            entity.ProductionDate = baseArticle.ProductionDate;

            // Copy identity if required and present
            if (!includeIdentity || baseArticle.Identity == null)
                return entity;

            entity.Identifier = baseArticle.Identity.Identifier;
            entity.NumberType = ((NumberIdentity)baseArticle.Identity).NumberType;
            return entity;
        }
    }
}