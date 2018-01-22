using Marvin.AbstractionLayer;
using Marvin.Model;
using Marvin.Products.Model;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Strategy for simple products
    /// </summary>
    public class DefaultProductStrategy<TProduct> : ProductStrategyBase, IProductTypeStrategy
        where TProduct : Product, new()
    {
        /// <summary>
        /// Create a new instance of the simple strategy
        /// </summary>
        public DefaultProductStrategy(bool skipArticles, bool includeParent)
        {
            TargetType = typeof(TProduct).Name;
            SkipArticles = skipArticles;
            IncludeParent = includeParent;
            Parts = new ILinkStrategy[0]; // Fill with empty array per default
        }

        /// <summary>
        /// Type of the product represented by this strategy
        /// </summary>
        public string TargetType { get; }

        ///
        public bool IncludeParent { get; }

        /// <summary>
        /// Active strategies for the parts of the product
        /// </summary>
        public ILinkStrategy[] Parts { get; protected set; }

        /// <summary>
        /// Load simple product and set basic properties
        /// </summary>
        public virtual IProduct LoadProduct(IUnitOfWork uow, ProductEntity entity)
        {
            var result = new TProduct();

            CopyToProduct(entity, result);

            return result;
        }

        /// <summary>
        /// Save simple product and persist basic properties
        /// </summary>
        public virtual ProductEntity SaveProduct(IUnitOfWork uow, IProduct product)
        {
            var entity = GetProductEntity(uow, product);

            CreateVersion(uow.GetRepository<IProductPropertiesRepository>(), product, entity);

            return entity;
        }

        /// <summary>
        /// Flag if articles are not persisted and shall be skipped
        /// </summary>
        public bool SkipArticles { get; }

        /// <summary>
        /// Per default all articles shall be created based on the products part link
        /// </summary>
        public virtual PartSourceStrategy ArticleCreation => PartSourceStrategy.FromPartlink;

        /// <summary>
        /// Copy basic properties to article
        /// </summary>
        public virtual void LoadArticle(IUnitOfWork uow, ArticleEntity entity, Article article)
        {
            CopyToArticle(entity, article);
        }

        /// <summary>
        /// Save basic article properties
        /// </summary>
        public virtual ArticleEntity SaveArticle(IUnitOfWork uow, Article article)
        {
            var entity = GetArticleEntity(uow, article);
            CopyToArticleEntity(article, entity, true);
            return entity;
        }
    }
}