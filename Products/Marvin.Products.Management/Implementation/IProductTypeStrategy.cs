using Marvin.AbstractionLayer;
using Marvin.Model;
using Marvin.Products.Model;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Strategy methods for a certain resource type
    /// </summary>
    public interface IProductTypeStrategy
    {
        /// <summary>
        /// Target product type of this strategy
        /// </summary>
        string TargetType { get; }

        /// <summary>
        /// Flag if the parent shall be loaded when instances of
        /// this type are loaded directly. This is usually the case
        /// for sets and 1on1 relations between parent and part.
        /// </summary>
        bool IncludeParent { get; }

        /// <summary>
        /// Get part descriptions for the product
        /// </summary>
        /// <returns></returns>
        ILinkStrategy[] Parts { get; }

        /// <summary>
        /// Load product from entity
        /// </summary>
        IProduct LoadProduct(IUnitOfWork uow, ProductEntity entity);

        /// <summary>
        /// Save product to database
        /// </summary>
        ProductEntity SaveProduct(IUnitOfWork uow, IProduct product);

        /// <summary>
        /// Flag if articles of this product type shall be skipped when loading or saving instances
        /// </summary>
        bool SkipArticles { get; }

        /// <summary>
        /// Load additional article properties from entity and write them to the business object
        /// </summary>
        void LoadArticle(IUnitOfWork uow, ArticleEntity entity, Article article);

        /// <summary>
        /// Save article instance to database
        /// </summary>
        ArticleEntity SaveArticle(IUnitOfWork uow, Article article);
    }
}