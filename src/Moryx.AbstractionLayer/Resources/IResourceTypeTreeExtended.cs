// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources;

// ToDo: Merge in MORYX 12
/// <summary>
/// Facade interface to resolve type information of resources and resource proxies
/// </summary>
/// <remarks>
/// This interface is going to merge with the underlying <see cref="IResourceTypeTree"/> interface on the next Major
/// release.
/// </remarks>
public interface IResourceTypeTreeExtended : IResourceTypeTree
{
    /// <summary>
    /// Get the type node of the given resource or resource proxy
    /// </summary>
    /// <param name="resource">Resource or resource proxy retrieved from the resource management</param>
    /// <returns>The type node of the resource, or null if the resource is unknown to the type tree</returns>
    /// <exception cref="ProxyDetachedException">The proxy was detached from its resource</exception>
    IResourceTypeNode GetNode(IResource resource);
}
