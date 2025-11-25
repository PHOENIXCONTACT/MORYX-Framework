// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.AbstractionLayer.Products;

/// <summary>
/// Attribute to register resource initializers
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ProductImporterAttribute : PluginAttribute
{
    /// <summary>
    /// Creates a product importer registration
    /// </summary>
    /// <param name="name">Name of this registration</param>
    public ProductImporterAttribute(string name)
        : base(LifeCycle.Transient, typeof(IProductImporter))
    {
        Name = name;
    }
}
