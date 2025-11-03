// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Products;
using Moryx.Modules;
using Moryx.Products.Model;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Strategy methods for a certain product type
    /// </summary>
    public interface IProductTypeStrategy : IConfiguredPlugin<ProductTypeConfiguration>
    {
        /// <summary>
        /// Target type of this strategy
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Detect changes between business object and current state of the database
        /// </summary>
        bool HasChanged(ProductType current, IGenericColumns dbProperties);

        /// <summary>
        /// Write product properties to database generic columns
        /// </summary>
        void SaveType(ProductType source, IGenericColumns target);

        /// <summary>
        /// Load product from database information
        /// </summary>
        void LoadType(IGenericColumns source, ProductType target);

        /// <summary>
        /// Transform a product class selector to a database compatible expression
        /// </summary>
        Expression<Func<IGenericColumns, bool>> TransformSelector<TProduct>(Expression<Func<TProduct, bool>> selector);
    }

}
