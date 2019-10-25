using System;
using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.Container;
using Marvin.Model;
using Marvin.Modules;
using Marvin.Products.Model;
using Marvin.Serialization;
using Newtonsoft.Json;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Strategy for simple products
    /// </summary>
    [ExpectedConfig(typeof(GenericTypeConfiguration))]
    [StrategyConfiguration(typeof(IProductType), DerivedTypes = true)]
    [Plugin(LifeCycle.Transient, typeof(IProductTypeStrategy), Name = nameof(GenericTypeStrategy))]
    internal class GenericTypeStrategy : TypeStrategyBase<GenericTypeConfiguration>
    {
        /// <summary>
        /// Injected entity mapper
        /// </summary>
        public GenericEntityMapper<ProductType, IProductPartLink> EntityMapper { get; set; }

        /// <summary>
        /// Initialize the type strategy
        /// </summary>
        public override void Initialize(ProductTypeConfiguration config)
        {
            base.Initialize(config);
            
            EntityMapper.Initialize(TargetType, Config);
        }

        public override bool HasChanged(IProductType current, IGenericColumns dbProperties)
        {
            return EntityMapper.HasChanged(dbProperties, current);
        }

        public override void SaveType(IProductType source, IGenericColumns target)
        {
            EntityMapper.WriteValue(source, target);
        }

        public override void LoadType(IGenericColumns source, IProductType target)
        {
            EntityMapper.ReadValue(source, target);
        }
    }
}