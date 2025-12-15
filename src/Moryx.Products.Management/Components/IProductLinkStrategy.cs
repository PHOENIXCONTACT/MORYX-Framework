// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Modules;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Interface to easily access
    /// </summary>
    public interface IProductLinkStrategy : IAsyncConfiguredInitializable<ProductLinkConfiguration>
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
        Task LoadPartLinkAsync(IGenericColumns linkEntity, ProductPartLink target, CancellationToken cancellationToken);

        /// <summary>
        /// Save part link
        /// </summary>
        Task SavePartLinkAsync(ProductPartLink source, IGenericColumns target, CancellationToken cancellationToken);

        /// <summary>
        /// A link between two products was removed, remove the link as well
        /// </summary>
        Task DeletePartLinkAsync(IReadOnlyList<IGenericColumns> deprecatedEntities, CancellationToken cancellationToken);
    }
}
