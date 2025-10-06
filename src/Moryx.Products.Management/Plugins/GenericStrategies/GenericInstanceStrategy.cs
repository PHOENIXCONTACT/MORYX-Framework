// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Products.Model;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Generic strategy for product instances
    /// </summary>
    [ExpectedConfig(typeof(GenericInstanceConfiguration))]
    [StrategyConfiguration(typeof(ProductInstance), DerivedTypes = true)]
    [Plugin(LifeCycle.Transient, typeof(IProductInstanceStrategy), Name = nameof(GenericInstanceStrategy))]
    internal class GenericInstanceStrategy : InstanceStrategyBase<GenericInstanceConfiguration>
    {
        /// <summary>
        /// Injected entity mapper
        /// </summary>
        public GenericEntityMapper<ProductInstance, ProductInstance> EntityMapper { get; set; }

        /// <summary>
        /// Initialize the type strategy
        /// </summary>
        public override void Initialize(ProductInstanceConfiguration config)
        {
            base.Initialize(config);

            EntityMapper.Initialize(TargetType, Config);
        }

        public override Expression<Func<IGenericColumns, bool>> TransformSelector<TInstance>(Expression<Func<TInstance, bool>> selector)
        {
            return EntityMapper.TransformSelector(selector);
        }

        public override void SaveInstance(ProductInstance source, IGenericColumns target)
        {
            EntityMapper.WriteValue(source, target);
        }

        public override void LoadInstance(IGenericColumns source, ProductInstance target)
        {
            EntityMapper.ReadValue(source, target);
        }
    }
}
