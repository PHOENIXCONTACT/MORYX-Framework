using System;
using System.Collections.Generic;
using Marvin.Workflows;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Merged facade for products and articles
    /// </summary>
    public interface IProductManagement : IRecipeProvider, IWorkplans
    {
        /// <summary>
        /// Get all products
        /// </summary>
        IEnumerable<IProduct> GetAllProducts();

        /// <summary>
        /// Load product instance by id
        /// </summary>
        IProduct GetProduct(long id);

        /// <summary>
        /// Load product by identity
        /// </summary>
        IProduct GetProduct(ProductIdentity identity);

        /// <summary>
        /// Event raised when a product changed
        /// </summary>
        event EventHandler<IProduct> ProductChanged; 

        /// <summary>
        /// Save a product to the database
        /// </summary>
        long SaveProduct(IProduct modifiedInstance);

        /// <summary>
        /// All importers and their parameters currently configured in the machine
        /// </summary>
        IDictionary<string, IImportParameters> Importers { get; }

        /// <summary>
        /// Import products for the given parameters with the named importer
        /// </summary>
        IProduct[] ImportProducts(string importerName, IImportParameters parameters);

            /// <summary>
        /// Retrieves the current recipe for this product
        /// </summary>
        IReadOnlyList<IProductRecipe> GetRecipes(IProduct product, RecipeClassification classification);

        /// <summary>
        /// Saves given recipe to the storage
        /// </summary>
        long SaveRecipe(IProductRecipe recipe);

        /// <summary>
        /// Create an article instance of given product
        /// </summary>
        /// <param name="product">Product to instanciate</param>
        /// <returns>Unsaved instance</returns>
        Article CreateInstance(IProduct product);

        /// <summary>
        /// Create an article instance of given product
        /// </summary>
        /// <param name="product">Product to instanciate</param>
        /// <param name="save">Flag if new instance should already be saved</param>
        /// <returns>New instance</returns>
        Article CreateInstance(IProduct product, bool save);

        /// <summary>
        /// Get an article with the given id.
        /// </summary>
        /// <param name="id">The id for the article which should be searched for.</param>
        /// <returns>The article with the id when it exists.</returns>
        Article GetArticle(long id);

        /// <summary>
        /// Updates the database from the article instance
        /// </summary>
        void SaveArticle(Article article);

        /// <summary>
        /// Updates the database from the article instance
        /// </summary>
        void SaveArticles(Article[] articles);

        /// <summary>
        /// Gets a list of articles by a given state
        /// </summary>
        IEnumerable<Article> GetArticles(ArticleState state);

        /// <summary>
        /// Load articles using combined bit flags
        /// </summary>
        IEnumerable<Article> GetArticles(int combinedState);
    }
}