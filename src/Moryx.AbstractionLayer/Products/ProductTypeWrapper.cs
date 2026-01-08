// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.AbstractionLayer.Products;

public class ProductTypeWrapper
{
    public string Identifier { get; private set; }
    public Func<ProductType> Constructor { get; private set; }

    public Dictionary<string, Func<ProductPartLink>> PartLinkConstructors { get; private set; }

    public List<PropertyInfo> Properties { get; private set; }

    public IEnumerable<PropertyInfo> PartLinks { get; private set; }

    public ProductTypeWrapper(string identifier, Func<ProductType> constructor, Dictionary<string, Func<ProductPartLink>> partLinkConstructors, List<PropertyInfo> properties)
    {
        Identifier = identifier;
        Constructor = constructor;
        PartLinkConstructors = partLinkConstructors;
        Properties = properties;
        PartLinks = properties.Where(p => typeof(ProductPartLink).IsAssignableFrom(p.PropertyType) || typeof(IEnumerable<ProductPartLink>).IsAssignableFrom(p.PropertyType));
    }
}