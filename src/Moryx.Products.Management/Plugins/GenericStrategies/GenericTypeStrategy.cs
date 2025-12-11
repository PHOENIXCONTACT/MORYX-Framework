// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Strategy for simple products
    /// </summary>
    [ExpectedConfig(typeof(GenericTypeConfiguration))]
    [StrategyConfiguration(typeof(ProductType), DerivedTypes = true)]
    [Plugin(LifeCycle.Transient, typeof(IProductTypeStrategy), Name = nameof(GenericTypeStrategy))]
    internal class GenericTypeStrategy : TypeStrategyBase<GenericTypeConfiguration>, IProductTypeStrategy
    {
        /// <summary>
        /// Injected entity mapper
        /// </summary>
        public GenericEntityMapper<ProductType, ProductPartLink> EntityMapper { get; set; }

        /// <summary>
        /// Initialize the type strategy
        /// </summary>
        public override void Initialize(ProductTypeConfiguration config)
        {
            base.Initialize(config);

            EntityMapper.Initialize(TargetType, Config);
        }

        public override Expression<Func<IGenericColumns, bool>> TransformSelector<TProduct>(Expression<Func<TProduct, bool>> selector)
        {
            return EntityMapper.TransformSelector(selector);
        }

        public override bool HasChanged(ProductType current, IGenericColumns dbProperties)
        {
            return EntityMapper.HasChanged(dbProperties, current);
        }

        public override Task SaveTypeAsync(ProductType source, IGenericColumns target)
        {
            EntityMapper.WriteValue(source, target);
            return Task.CompletedTask;
        }

        public override Task LoadTypeAsync(IGenericColumns source, ProductType target)
        {
            EntityMapper.ReadValue(source, target);
            return Task.CompletedTask;
        }
    }
}
