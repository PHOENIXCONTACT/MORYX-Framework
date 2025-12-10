// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Management.NullStrategies
{
    /// <summary>
    /// Strategy for product instances that should not be saved to the database
    /// </summary>
    [PropertylessStrategyConfiguration(typeof(ProductInstance), DerivedTypes = true)]
    [Plugin(LifeCycle.Transient, typeof(IProductInstanceStrategy), Name = nameof(SkipInstancesStrategy))]
    public class SkipInstancesStrategy : InstanceStrategyBase
    {
        /// <summary>
        /// Create new instance of <see cref="SkipInstancesStrategy"/>
        /// </summary>
        public SkipInstancesStrategy() : base(true)
        {
        }

        /// <inheritdoc />
        public override void Initialize(ProductInstanceConfiguration config)
        {
            base.Initialize(config);

            SkipInstances = true;
        }

        /// <inheritdoc />
        public override Expression<Func<IGenericColumns, bool>> TransformSelector<TInstance>(Expression<Func<TInstance, bool>> selector)
        {
            return c => false;
        }

        /// <inheritdoc />
        public sealed override Task SaveInstanceAsync(ProductInstance source, IGenericColumns target)
        {
            // Not necessary
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public sealed override Task LoadInstanceAsync(IGenericColumns source, ProductInstance target)
        {
            // Nothing
            return Task.CompletedTask;
        }
    }
}
