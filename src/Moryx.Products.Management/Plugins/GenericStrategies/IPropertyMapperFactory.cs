// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Container;

namespace Moryx.Products.Management
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
