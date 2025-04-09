// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Products.Model;

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
        public sealed override void SaveInstance(ProductInstance source, IGenericColumns target)
        {
            // Not necessary
        }

        /// <inheritdoc />
        public sealed override void LoadInstance(IGenericColumns source, ProductInstance target)
        {
            // Nothing
        }
    }
}
