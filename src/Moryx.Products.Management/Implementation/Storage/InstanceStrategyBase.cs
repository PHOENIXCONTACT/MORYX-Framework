// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Products;
using Moryx.Products.Management.Model;
using Moryx.Tools;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Non-generic base class for <see cref="IProductInstanceStrategy"/>
    /// </summary>
    public abstract class InstanceStrategyBase : InstanceStrategyBase<ProductInstanceConfiguration>
    {
        /// <summary>
        /// Empty constructor for derived types
        /// </summary>
        protected InstanceStrategyBase()
        {
        }

        /// <summary>
        /// Create a new instance of the simple strategy
        /// </summary>
        protected InstanceStrategyBase(bool skipArticles) : base(skipArticles)
        {
        }
    }

    /// <summary>
    /// Base class for all <see cref="IProductInstanceStrategy"/>
    /// </summary>
    /// <typeparam name="TConfig"></typeparam>
    public abstract class InstanceStrategyBase<TConfig> : StrategyBase<TConfig, ProductInstanceConfiguration>, IProductInstanceStrategy
        where TConfig : ProductInstanceConfiguration
    {
        /// <inheritdoc />
        public bool SkipInstances { get; protected set; }

        /// <summary>
        /// Empty constructor for derived types
        /// </summary>
        protected InstanceStrategyBase()
        {
        }

        /// <summary>
        /// Create a new instance of the simple strategy
        /// </summary>
        protected InstanceStrategyBase(bool skipArticles) : this()
        {
            SkipInstances = skipArticles;
        }

        /// <inheritdoc />
        public override void Initialize(ProductInstanceConfiguration config)
        {
            base.Initialize(config);

            TargetType = ReflectionTool.GetPublicClasses<ProductInstance>(p => p.FullName == config.TargetType).FirstOrDefault();
        }

        /// <inheritdoc />
        public abstract Expression<Func<IGenericColumns, bool>> TransformSelector<TInstance>(Expression<Func<TInstance, bool>> selector);

        /// <inheritdoc />
        public abstract Task SaveInstanceAsync(ProductInstance source, IGenericColumns target);

        /// <inheritdoc />
        public abstract Task LoadInstanceAsync(IGenericColumns source, ProductInstance target);
    }
}
