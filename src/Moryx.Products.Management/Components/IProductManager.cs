// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;
using Moryx.Modules;
using Moryx.Products.Management.Importers;

namespace Moryx.Products.Management
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
        IReadOnlyList<IProductType> LoadTypes(ProductQuery query);

        /// <summary>
        /// Load product instance by id
        /// </summary>
        IProductType LoadType(long id);

        /// <summary>
        /// Load product by identity
        /// </summary>
        IProductType LoadType(ProductIdentity identity);

        /// <summary>
        /// Create a new product for the given group type
        /// </summary>
        IProductType CreateType(string type);

        /// <summary>
        /// Event raised when a product changed
        /// </summary>
        event EventHandler<IProductType> TypeChanged;

        /// <summary>
        /// Save a product to the database
        /// </summary>
        long SaveType(IProductType modifiedInstance);

        /// <summary>
        /// Create revision of this product with provided revision number
        /// </summary>
        IProductType Duplicate(ProductType source, ProductIdentity identity);

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
        bool DeleteType(long productId);

        /// <summary>
        /// Create an instance of given product
        /// </summary>
        /// <param name="productType">Product to instantiate</param>
        /// <param name="save">Flag if new instance should already be saved</param>
        /// <returns>New instance</returns>
        ProductInstance CreateInstance(IProductType productType, bool save);

        /// <summary>
        /// Updates the database from the instance
        /// </summary>
        void SaveInstances(params ProductInstance[] productInstances);

        /// <summary>
        /// Get instances with the given ids.
        /// </summary>
        /// <param name="ids">The IDs of instances that should be loaded</param>
        /// <returns>The instance with the id when it exists.</returns>
        IReadOnlyList<ProductInstance> GetInstances(params long[] ids);

        /// <summary>
        /// Get all instances that match a certain expression
        /// </summary>
        IReadOnlyList<TInstance> GetInstances<TInstance>(Expression<Func<TInstance, bool>> selector);
    }
}
