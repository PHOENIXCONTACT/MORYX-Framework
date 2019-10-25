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
        /// Get products based on a query
        /// </summary>
        IReadOnlyList<IProductType> GetTypes(ProductQuery query);

        /// <summary>
        /// Load product instance by id
        /// </summary>
        IProductType GetType(long id);

        /// <summary>
        /// Load product by identity
        /// </summary>
        IProductType GetType(ProductIdentity identity);

        /// <summary>
        /// Event raised when a product changed
        /// </summary>
        event EventHandler<IProductType> TypeChanged;

        /// <summary>
        /// Duplicate a product under a new identity
        /// </summary>
        /// <exception cref="IdentityConflictException">Thrown when the new identity causes conflicts</exception>
        IProductType Duplicate(IProductType template, ProductIdentity newIdentity);

        /// <summary>
        /// Save a product to the database
        /// </summary>
        long SaveType(IProductType modifiedInstance);

        /// <summary>
        /// All importers and their parameters currently configured in the machine
        /// </summary>
        IDictionary<string, IImportParameters> Importers { get; }

        /// <summary>
        /// Import products for the given parameters with the named importer
        /// </summary>
        IReadOnlyList<IProductType> ImportTypes(string importerName, IImportParameters parameters);

            /// <summary>
        /// Retrieves the current recipe for this product
        /// </summary>
        IReadOnlyList<IProductRecipe> GetRecipes(IProductType productType, RecipeClassification classification);

        /// <summary>
        /// Saves given recipe to the storage
        /// </summary>
        long SaveRecipe(IProductRecipe recipe);

        /// <summary>
        /// Create an article instance of given product
        /// </summary>
        /// <param name="productType">Product to instanciate</param>
        /// <returns>Unsaved instance</returns>
        ProductInstance CreateInstance(IProductType productType);

        /// <summary>
        /// Create an article instance of given product
        /// </summary>
        /// <param name="productType">Product to instanciate</param>
        /// <param name="save">Flag if new instance should already be saved</param>
        /// <returns>New instance</returns>
        ProductInstance CreateInstance(IProductType productType, bool save);

        /// <summary>
        /// Get an article with the given id.
        /// </summary>
        /// <param name="id">The id for the article which should be searched for.</param>
        /// <returns>The article with the id when it exists.</returns>
        ProductInstance GetInstance(long id);

        /// <summary>
        /// Updates the database from the article instance
        /// </summary>
        void SaveInstance(ProductInstance productInstance);

        /// <summary>
        /// Updates the database from the article instance
        /// </summary>
        void SaveInstances(ProductInstance[] productInstances);

        /// <summary>
        /// Gets a list of articles by a given state
        /// </summary>
        IEnumerable<ProductInstance> GetInstances(ProductInstanceState state);

        /// <summary>
        /// Load articles using combined bit flags
        /// </summary>
        IEnumerable<ProductInstance> GetInstances(int combinedState);
    }
}