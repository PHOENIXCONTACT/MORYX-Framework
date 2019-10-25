using System.Collections.Generic;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Config interface for generic strategies
    /// </summary>
    internal interface IGenericMapperConfiguration : IPropertyMappedConfiguration
    {
        /// <summary>
        /// Column that should be used to store all non-configured properties as JSON
        /// </summary>
        string JsonColumn { get; }
    }
}