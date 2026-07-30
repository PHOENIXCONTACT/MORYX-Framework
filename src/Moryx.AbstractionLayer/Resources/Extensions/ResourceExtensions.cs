// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.AbstractionLayer.Resources;

/// <summary>
/// Extensions for resources
/// </summary>
public static class ResourceExtensions
{
    // ToDo: Should this also return the source type name in MORYX 12?
    /// <summary>
    /// Returns the string representation of the resource type
    /// </summary>
    /// <param name="resource">Source resource</param>
    /// <returns>Type identifier as string</returns>
    public static string ResourceType(this IResource resource)
    {
        return resource.GetType().FullName;
    }

    /// <summary>
    /// Proxy-aware method to retrieve the type of the <param name="resource"></param>.
    /// </summary>
    /// <param name="resource">Source resource</param>
    /// <returns>Original type of the resource</returns>
    public static Type GetResourceType(this IResource resource) =>
        resource.GetType().GetCustomAttribute<ProxySourceTypeAttribute>()?.ResourceType ?? resource.GetType();

    /// <param name="resource">Source resource</param>
    extension(Resource resource)
    {
        /// <summary>
        /// Recursive method to return the first resource that matches the <paramref name="conditionToMatch"/>
        /// while selecting the next resource to check using the <paramref name="methodToNavigate"/>.
        /// If the provided <paramref name="resource"/> matches the condition or is null it is returned.
        /// </summary>
        /// <param name="conditionToMatch">The condition to be matched by the returned resource</param>
        /// <param name="methodToNavigate">The method to navigate from a resource that does not match the condition to
        /// the next resource for a recursive check</param>
        /// <returns>The first resource that occurs in the recursion to match the condition, null otherwise</returns>
        public Resource GetFirstRelatedResource(Predicate<Resource> conditionToMatch, Func<Resource, Resource> methodToNavigate)
        {
            return resource is null || conditionToMatch(resource)
                ? resource
                : GetFirstRelatedResource(methodToNavigate(resource), conditionToMatch, methodToNavigate);
        }

        /// <summary>
        /// Recursive method to return the first resource that matches the <paramref name="conditionToMatch"/>
        /// while selecting the next resources to check using the <paramref name="methodToNavigate"/>.
        /// The order of traversing to the subsequent resources is equivalent to a Depth First Search execution.
        /// If the provided <paramref name="resource"/> matches the condition or is null it is returned.
        /// </summary>
        /// <param name="conditionToMatch">The condition to be matched by the returned resource</param>
        /// <param name="methodToNavigate">The method to navigate from a resource that does not match the condition to
        /// the next resources for a recursive check</param>
        /// <returns>The first resource that occurs in the recursion to match the condition, null otherwise</returns>
        public Resource GetFirstRelatedResource(Predicate<Resource> conditionToMatch, Func<Resource, IEnumerable<Resource>> methodToNavigate)
        {
            if (resource is null || conditionToMatch(resource))
            {
                return resource;
            }
            else
            {
                foreach (var subsequentResource in methodToNavigate(resource))
                {
                    var matchingResource = GetFirstRelatedResource(subsequentResource, conditionToMatch, methodToNavigate);
                    if (matchingResource is not null)
                    {
                        return matchingResource;
                    }
                }

                return null;
            }
        }
    }
}
