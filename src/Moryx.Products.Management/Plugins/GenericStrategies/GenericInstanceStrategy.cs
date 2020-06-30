// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Products;
using Marvin.Container;
using Marvin.Model;
using Marvin.Modules;
using Marvin.Products.Model;

namespace Marvin.Products.Management
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
