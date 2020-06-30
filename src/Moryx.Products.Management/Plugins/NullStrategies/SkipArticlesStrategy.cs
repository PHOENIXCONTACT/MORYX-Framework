// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Products.Model;

namespace Moryx.Products.Management.NullStrategies
{
    /// <summary>
    /// Strategiy for product instances that should not be saved to the database
    /// </summary>
    [PropertylessStrategyConfiguration(typeof(ProductInstance), DerivedTypes = true)]
    [Plugin(LifeCycle.Transient, typeof(IProductInstanceStrategy), Name = nameof(SkipArticlesStrategy))]
    public class SkipArticlesStrategy : InstanceStrategyBase
    {
        public SkipArticlesStrategy() : base(true)
        {
        }

        public override void Initialize(ProductInstanceConfiguration config)
        {
            base.Initialize(config);

            SkipInstances = true;
        }

        public sealed override void SaveInstance(ProductInstance source, IGenericColumns target)
        {
            // Not necessary
        }

        public sealed override void LoadInstance(IGenericColumns source, ProductInstance target)
        {
            // Nothing
        }
    }
}
