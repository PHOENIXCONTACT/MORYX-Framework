using System;
using Marvin.AbstractionLayer;
using Marvin.Model;
using Marvin.Modules;
using Marvin.Products.Model;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Strategy for loading and saving product instances
    /// </summary>
    public interface IProductInstanceStrategy : IConfiguredPlugin<ProductInstanceConfiguration>
    {
        /// <summary>
        /// Target type of this strategy
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Flag if instances of this product type shall be skipped when loading or saving instances
        /// </summary>
        bool SkipInstances { get; }

        /// <summary>
        /// Save article instance to database
        /// </summary>
        void SaveInstance(ProductInstance source, IGenericColumns target);

        /// <summary>
        /// Load additional article properties from entity and write them to the business object
        /// </summary>
        void LoadInstance(IGenericColumns source, ProductInstance target);
    }
}