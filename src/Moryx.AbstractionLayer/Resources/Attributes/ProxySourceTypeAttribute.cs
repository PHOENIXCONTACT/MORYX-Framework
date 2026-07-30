// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources;

/// <summary>
/// Attribute to decorate resource proxies providing the type information
/// from the source type of resource.
/// </summary>
/// <remarks>
/// Using the extension method <see cref="ResourceExtensions.GetResourceType"/> the source type of a resource (proxy)
/// can be retrieved.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ProxySourceTypeAttribute : Attribute
{
    /// <summary>
    /// The source type of the decorated resource
    /// </summary>
    public Type ResourceType { get; }

    /// <summary>
    /// Creates a new instance to decorate a resource proxy
    /// </summary>
    public ProxySourceTypeAttribute(Type resourceType) => ResourceType = resourceType;
}
