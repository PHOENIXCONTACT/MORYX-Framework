// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Marvin.AbstractionLayer;
using Marvin.Model;
using Marvin.Modules;
using Marvin.Products.Model;

namespace Marvin.Products.Management
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
        bool HasChanged(IProductType current, IGenericColumns dbProperties);

        /// <summary>
        /// Write product properties to database generic columns
        /// </summary>
        void SaveType(IProductType source, IGenericColumns target);

        /// <summary>
        /// Load product from database information
        /// </summary>
        void LoadType(IGenericColumns source, IProductType target);
    }
}
