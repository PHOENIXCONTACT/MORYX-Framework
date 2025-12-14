// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;
using Moryx.Modules;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Management component
    /// </summary>
    internal interface IProductManager : IAsyncPlugin
    {
        /// <summary>
        /// Returns all available product importers
        /// </summary>
        IProductImporter[] Importers { get; }

        /// <summary>
        /// Returns all products on this machine
        /// </summary>
        Task<IReadOnlyList<ProductType>> LoadTypes(ProductQuery query);

        /// <summary>
        /// Load types using filter expression
        /// </summary>
        Task<IReadOnlyList<TType>> LoadTypes<TType>(Expression<Func<TType, bool>> selector);

        /// <summary>
        /// Load product instance by id
        /// </summary>
        Task<ProductType> LoadType(long id);

        /// <summary>
        /// Load product by identity
        /// </summary>
        Task<ProductType> LoadType(IIdentity identity);

        /// <summary>
        /// Create a new product for the given group type
        /// </summary>
        ProductType CreateType(string type);

        /// <summary>
        /// Event raised when a product changed
        /// </summary>
        event EventHandler<ProductType> TypeChanged;

        /// <summary>
        /// Save a product to the database
        /// </summary>
        Task<long> SaveType(ProductType modifiedInstance);

        /// <summary>
        /// Create revision of this product with provided revision number
        /// </summary>
        Task<ProductType> Duplicate(ProductType source, IIdentity identity);

        /// <summary>
        /// Import the given file as a product to the database
        /// </summary>
        Task<ProductImportResult> Import(string importer, object parameters);

        /// <summary>
        /// Import GUID based
        /// </summary>
        ImportState ImportParallel(string importer, object parameters);

        /// <summary>
        /// Fetch import progress
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        ImportState ImportProgress(Guid session);

        /// <summary>
        /// Try to delete a product. If it is still used as a part in other products, it will return <c>false</c>
        /// </summary>
        /// <param name="productId">Id of the product that is deprecated and should be deleted.</param>
        /// <returns><value>True</value> if the product was removed, <value>false</value> otherwise</returns>
        Task<bool> DeleteType(long productId);

        /// <summary>
        /// Create an instance of given product
        /// </summary>
        /// <param name="productType">Product to instantiate</param>
        /// <param name="save">Flag if new instance should already be saved</param>
        /// <returns>New instance</returns>
        Task<ProductInstance> CreateInstance(ProductType productType, bool save);

        /// <summary>
        /// Updates the database from the instance
        /// </summary>
        Task SaveInstances(params ProductInstance[] productInstances);

        /// <summary>
        /// Get instances with the given ids.
        /// </summary>
        /// <param name="ids">The IDs of instances that should be loaded</param>
        /// <returns>The instance with the id when it exists.</returns>
        Task<IReadOnlyList<ProductInstance>> GetInstances(long[] ids);

        /// <summary>
        /// Get all instances that match a certain expression
        /// </summary>
        Task<IReadOnlyList<TInstance>> GetInstances<TInstance>(Expression<Func<TInstance, bool>> selector);

        /// <summary>
        /// Return type wrapper to a type
        /// </summary>
        /// <param name="typeName">Full name of the type</param>
        /// <returns></returns>
        ProductTypeWrapper GetTypeWrapper(string typeName);
    }
}
