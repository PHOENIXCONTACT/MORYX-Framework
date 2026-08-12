// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Material.IntegrationTests;

public class ResourceTypeNodeMock(string name, Type resourceType, bool creatable) : IResourceTypeNode
{
    public string Name { get; private set; } = name;

    // TODO: Why is IResourceTypeNode.ResourceType public set
    public Type ResourceType { get; set; } = resourceType;

    public bool Creatable { get; private set; } = creatable;

    public MethodInfo[] Constructors => throw new NotImplementedException();

    public IResourceTypeNode BaseType => throw new NotImplementedException();

    public IEnumerable<IResourceTypeNode> DerivedTypes => throw new NotImplementedException();

    public IEnumerable<PropertyInfo> References => throw new NotImplementedException();

    public IEnumerable<PropertyInfo> PropertiesOfResourceType => throw new NotImplementedException();

    public Dictionary<string, List<Type>> ReferenceOverrides => throw new NotImplementedException();
}
