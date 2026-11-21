// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources;

/// <summary>
/// Component that manages the resource type tree
/// </summary>
public interface IResourceTypeTree
{
    /// <summary>
    /// Type tree starting at <see cref="Resource"/>
    /// </summary>
    IResourceTypeNode RootType { get; }

    /// <summary>
    /// Indexed selection of a resource type
    /// </summary>
    IResourceTypeNode this[string typeName] { get; }

    /// <summary>
    /// Find all types that implement the given type constraint
    /// </summary>
    IEnumerable<IResourceTypeNode> SupportedTypes(Type constraint);

    /// <summary>
    /// Find all resource type trees that implement the given type constraints
    /// </summary>
    IEnumerable<IResourceTypeNode> SupportedTypes(ICollection<Type> constraints);

    /// <summary>
    /// Get the type node of the given resource or resource proxy
    /// </summary>
    /// <param name="resource">Resource or resource proxy retrieved from the resource management</param>
    /// <returns>The type node of the resource, or null if the resource is unknown to the type tree</returns>
    /// <exception cref="ProxyDetachedException">The proxy was detached from its resource</exception>
    IResourceTypeNode GetNode(IResource resource);
}
