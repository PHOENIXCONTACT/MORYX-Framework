using System;
using Marvin.Container;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Factory to create instances of <see cref="IPropertyMapper"/>
    /// </summary>
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface IPropertyMapperFactory
    {
        /// <summary>
        /// Create a new mapper instance from config
        /// </summary>
        IPropertyMapper Create(PropertyMapperConfig config, Type targetType);

        /// <summary>
        /// Destroy a mapper instance
        /// </summary>
        void Destroy(IPropertyMapper instance);
    }
}