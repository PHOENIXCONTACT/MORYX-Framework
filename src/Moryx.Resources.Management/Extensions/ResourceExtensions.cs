// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management;

internal static class ResourceExtensions
{
    public static TResource Proxify<TResource>(this TResource source, IResourceTypeController typeController)
        where TResource : class, IResource
    {
        return (TResource)typeController.GetProxy(source as Resource);
    }

    public static IEnumerable<TResource> Proxify<TResource>(this IEnumerable<TResource> source, IResourceTypeController typeController)
        where TResource : class, IResource
    {
        return source.Select(r => r.Proxify(typeController));
    }

    public static string ResourceType(this Type resourceType) => resourceType.FullName;

    public static string ResourceType(this IResource resource) => resource.GetType().FullName;
}
