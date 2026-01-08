// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Products.Management.Model;

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
        public override async Task InitializeAsync(ProductInstanceConfiguration config, CancellationToken cancellationToken = default)
        {
            await base.InitializeAsync(config, cancellationToken);

            EntityMapper.Initialize(TargetType, Config);
        }

        public override Expression<Func<IGenericColumns, bool>> TransformSelector<TInstance>(Expression<Func<TInstance, bool>> selector)
        {
            return EntityMapper.TransformSelector(selector);
        }

        public override Task SaveInstanceAsync(ProductInstance source, IGenericColumns target, CancellationToken cancellationToken)
        {
            EntityMapper.WriteValue(source, target);
            return Task.CompletedTask;
        }

        public override Task LoadInstanceAsync(IGenericColumns source, ProductInstance target, CancellationToken cancellationToken)
        {
            EntityMapper.ReadValue(source, target);
            return Task.CompletedTask;
        }
    }
}
