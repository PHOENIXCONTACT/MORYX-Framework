// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Modules;
using Moryx.Products.Model;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Interface to easily access
    /// </summary>
    public interface IProductLinkStrategy : IConfiguredPlugin<ProductLinkConfiguration>
    {
        /// <summary>
        /// Target type of this strategy
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Name of the parts property
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Strategy how product instance parts are created during loading
        /// </summary>
        PartSourceStrategy PartCreation { get; }

        /// <summary>
        /// Load typed object and set on product
        /// </summary>
        void LoadPartLink(IGenericColumns linkEntity, IProductPartLink target);

        /// <summary>
        /// Save part link
        /// </summary>
        void SavePartLink(IProductPartLink source, IGenericColumns target);

        /// <summary>
        /// A link between two products was removed, remove the link as well
        /// </summary>
        void DeletePartLink(IReadOnlyList<IGenericColumns> deprecatedEntities);
    }
}
