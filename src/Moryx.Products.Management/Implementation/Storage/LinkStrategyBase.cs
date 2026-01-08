// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Products.Management.Model;
using Moryx.Tools;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Default part accessor without model merge
    /// </summary>
    public abstract class LinkStrategyBase : LinkStrategyBase<ProductLinkConfiguration>
    {
    }

    /// <summary>
    /// Default part accessor without model merge
    /// </summary>
    public abstract class LinkStrategyBase<TConfig> : StrategyBase<TConfig, ProductLinkConfiguration>, IProductLinkStrategy
        where TConfig : ProductLinkConfiguration
    {
        /// <summary>
        /// Name of the parts property
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Strategy to determine how article instances are loaded as parts
        /// </summary>
        public PartSourceStrategy PartCreation { get; private set; }

        /// <inheritdoc />
        public override async Task InitializeAsync(ProductLinkConfiguration config, CancellationToken cancellationToken = default)
        {
            await base.InitializeAsync(config, cancellationToken);

            PropertyName = config.PartName;
            PartCreation = config.PartCreation;

            TargetType = ReflectionTool.GetPublicClasses<ProductType>(p => p.FullName == config.TargetType).FirstOrDefault();
        }

        /// <inheritdoc />
        public abstract Task LoadPartLinkAsync(IGenericColumns linkEntity, ProductPartLink target, CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract Task SavePartLinkAsync(ProductPartLink source, IGenericColumns target, CancellationToken cancellationToken);

        /// <inheritdoc />
        public virtual Task DeletePartLinkAsync(IReadOnlyList<IGenericColumns> deprecatedEntities, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
