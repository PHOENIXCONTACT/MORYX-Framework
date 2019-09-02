using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer;
using Marvin.Modules;
using Marvin.Products.Management.Importers;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Management component
    /// </summary>
    internal interface IProductManager : IPlugin
    {
        /// <summary>
        /// Returns all available product importers
        /// </summary>
        IProductImporter[] Importers { get; }

        /// <summary>
        /// Returns all products on this machine
        /// </summary>
        IReadOnlyList<IProduct> GetProducts(ProductQuery query);

        /// <summary>
        /// Load product instance by id
        /// </summary>
        IProduct GetProduct(long id);

        /// <summary>
        /// Load product by identity
        /// </summary>
        IProduct GetProduct(ProductIdentity identity);

        /// <summary>
        /// Create a new product for the given group type
        /// </summary>
        IProduct Create(string type);

        /// <summary>
        /// Event raised when a product changed
        /// </summary>
        event EventHandler<IProduct> ProductChanged;

        /// <summary>
        /// Save a product to the database
        /// </summary>
        long Save(IProduct modifiedInstance);

        /// <summary>
        /// Create revision of this product with provided revision number
        /// </summary>
        IProduct Duplicate(long sourceId, ProductIdentity identity);

        /// <summary>
        /// Import the given file as a product to the database
        /// </summary>
        IReadOnlyList<IProduct> ImportProducts(string importer, IImportParameters parameters);

        /// <summary>
        /// Try to delete a product. If it is still used as a part in other products, it will return <c>false</c>
        /// </summary>
        /// <param name="productId">Id of the product that is deprecated and should be deleted.</param>
        /// <returns><value>True</value> if the product was removed, <value>false</value> otherwise</returns>
        bool DeleteProduct(long productId);

        /// <summary>
        /// Create an article instance of given product
        /// </summary>
        /// <param name="product">Product to instantiate</param>
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
        /// Gets a list of articles by a given state
        /// </summary>
        IEnumerable<Article> GetArticles(ArticleState state);

        /// <summary>
        /// Load articles using combined bit flags
        /// </summary>
        IEnumerable<Article> GetArticles(int state);

        /// <summary>
        /// Updates the database from the article instance
        /// </summary>
        void SaveArticles(params Article[] articles);
    }
}