// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Products.Management
{
    /// <summary>
    /// Interface for <see cref="IProductStrategyConfiguration"/> that contains property mapper configuration
    /// </summary>
    public interface IPropertyMappedConfiguration
    {
        /// <summary>
        /// Configuration for individual property mappers
        /// </summary>
        List<PropertyMapperConfig> PropertyConfigs { get; }
    }
}
