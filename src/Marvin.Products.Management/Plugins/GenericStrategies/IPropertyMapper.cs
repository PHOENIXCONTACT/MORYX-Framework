// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Modules;
using Marvin.Products.Model;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Strategy to map a single property into the database
    /// </summary>
    public interface IPropertyMapper : IConfiguredPlugin<PropertyMapperConfig>, IGenericMapper
    {
        /// <summary>
        /// Name of the property represented by this mapper
        /// </summary>
        string PropertyName { get; }
    }
}
