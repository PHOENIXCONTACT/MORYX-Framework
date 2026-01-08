// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management;

internal static class ResourceExtensions
{
    public static TResource Proxify<TResource>(this TResource source, IResourceTypeController typeController)
        where TResource : class, IResource
    {
        if (ResourceTypeController.IsGenericResourceInterface(typeof(TResource)))
            throw new NotSupportedException("Generic resource interfaces are not supported through the facade!");

        return (TResource)typeController.GetProxy(source as Resource);
    }

    public static IEnumerable<TResource> Proxify<TResource>(this IEnumerable<TResource> source, IResourceTypeController typeController)
        where TResource : class, IResource
    {
        return source.Select(r => r.Proxify(typeController));
    }

    /// <summary>
    /// Returns the resource identifier based on <see cref="Type"/>
    /// </summary>
    /// <param name="resourceType">Type of the resource</param>
    /// <returns>Identifier for the type</returns>
    internal static string ResourceType(this Type resourceType) => resourceType.FullName;
}