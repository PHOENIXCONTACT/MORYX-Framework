using System.Collections.Generic;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Interface for <see cref="IProductStrategyConfiguation"/> that contains property mapper configuration
    /// </summary>
    public interface IPropertyMappedConfiguration
    {
        /// <summary>
        /// Configuration for individual property mappers
        /// </summary>
        List<PropertyMapperConfig> PropertyConfigs { get; }
    }
}