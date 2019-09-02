using System.Collections.Generic;
using Marvin.AbstractionLayer;
using Marvin.Model;
using Marvin.Products.Model;

namespace Marvin.Products.Management
{
    /// <summary>
    /// API for the application specific product storage
    /// </summary>
    public interface IProductStorage
    {
        /// <summary>
        /// Load product instance by id
        /// </summary>
        IProduct LoadProduct(long id);

        /// <summary>
        /// Load product by identity. This method supports loading a products latest revision
        /// </summary>
        IProduct LoadProduct(ProductIdentity identity);

        /// <summary>
        /// Transform a given a product entity
        /// </summary>
        IProduct TransformProduct(IUnitOfWork context, ProductEntity entity, bool full);

        /// <summary>
        /// Save a product to the storage
        /// </summary>
        long SaveProduct(IProduct modifiedInstance);

        /// <summary>
        /// Get an article with the given id.
        /// </summary>
        /// <param name="id">The id for the article which should be searched for.</param>
        /// <returns>The article with the id when it exists.</returns>
        Article LoadArticle(long id);

        /// <summary>
        /// Gets a list of articles by a given state
        /// </summary>
        IEnumerable<Article> LoadArticles(ArticleState state);

        /// <summary>
        /// Load articles using combined bit flags
        /// </summary>
        IEnumerable<Article> LoadArticles(int combinedState);

        /// <summary>
        /// Updates the database from the article instance
        /// </summary>
        void SaveArticles(Article[] article);

        /// <summary>
        /// Loads a recipe from the storage
        /// </summary>
        IProductRecipe LoadRecipe(long recipeId);

        /// <summary>
        /// Loads all recipes from the storage.
        /// </summary>
        IReadOnlyList<IProductRecipe> LoadRecipes(long productId, RecipeClassification classification);

        /// <summary>
        /// Saves the recipe of the product
        /// </summary>
        long SaveRecipe(IProductRecipe recipe);

        /// <summary>
        /// Save multiple recipes at once
        /// </summary>
        void SaveRecipes(long productId, ICollection<IProductRecipe> recipes);
    }
}