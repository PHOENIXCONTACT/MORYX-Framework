// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.Model;
using Moryx.Modules;
using Moryx.Products.Model;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Strategy for loading and saving product instances
    /// </summary>
    public interface IProductInstanceStrategy : IConfiguredPlugin<ProductInstanceConfiguration>
    {
        /// <summary>
        /// Target type of this strategy
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Flag if instances of this product type shall be skipped when loading or saving instances
        /// </summary>
        bool SkipInstances { get; }

        /// <summary>
        /// Save instance to database
        /// </summary>
        void SaveInstance(ProductInstance source, IGenericColumns target);

        /// <summary>
        /// Load additional instance properties from entity and write them to the business object
        /// </summary>
        void LoadInstance(IGenericColumns source, ProductInstance target);
    }
}
