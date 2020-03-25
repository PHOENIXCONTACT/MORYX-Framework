// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Products;
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
        IProductType Duplicate(long sourceId, ProductIdentity identity);

        /// <summary>
        /// Import the given file as a product to the database
        /// </summary>
        IReadOnlyList<IProductType> ImportTypes(string importer, IImportParameters parameters);

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
        /// Get an instance with the given id.
        /// </summary>
        /// <param name="id">The id for the instance which should be searched for.</param>
        /// <returns>The instance with the id when it exists.</returns>
        ProductInstance GetInstance(long id);

        /// <summary>
        /// Gets a list of instances by a given state
        /// </summary>
        IEnumerable<ProductInstance> GetInstances(ProductInstanceState state);

        /// <summary>
        /// Load instances using combined bit flags
        /// </summary>
        IEnumerable<ProductInstance> GetInstances(int state);

        /// <summary>
        /// Updates the database from the instance
        /// </summary>
        void SaveInstances(params ProductInstance[] productInstances);
    }
}
