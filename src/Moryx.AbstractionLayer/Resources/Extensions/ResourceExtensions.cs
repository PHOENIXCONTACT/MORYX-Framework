// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Extensions for resources
    /// </summary>
    public static class ResourceExtensions
    {
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
        /// Recursive method to return the first resource that matches the <paramref name="conditionToMatch"/> 
        /// while selecting the next resource to check using the <paramref name="methodToNavigate"/>.
        /// If the provided <paramref name="resource"/> matches the condition or is null it is returned.
        /// </summary>
        /// <param name="resource">Source resource</param>
        /// <param name="conditionToMatch">The condition to be matched by the returned resource</param>
        /// <param name="methodToNavigate">The method to navigate from a resource that does not match the condition to 
        /// the next resource for a recursive check</param>
        /// <returns>The first resource that occurs in the recursion to match the condition, null otherwise</returns>
        public static Resource GetFirstRelatedResource(this Resource resource, Predicate<Resource> conditionToMatch, Func<Resource, Resource> methodToNavigate)
        {
            if (resource is null || conditionToMatch(resource))
                return resource;
            else
                return GetFirstRelatedResource(methodToNavigate(resource), conditionToMatch, methodToNavigate);
        }


        /// <summary>
        /// Recursive method to return the first resource that matches the <paramref name="conditionToMatch"/> 
        /// while selecting the next resources to check using the <paramref name="methodToNavigate"/>.
        /// The order of traversing to the subsequent resources is equivalent to a Depth First Search execution.
        /// If the provided <paramref name="resource"/> matches the condition or is null it is returned.
        /// </summary>
        /// <param name="resource">Source resource</param>
        /// <param name="conditionToMatch">The condition to be matched by the returned resource</param>
        /// <param name="methodToNavigate">The method to navigate from a resource that does not match the condition to 
        /// the next resources for a recursive check</param>
        /// <returns>The first resource that occurs in the recursion to match the condition, null otherwise</returns>
        public static Resource GetFirstRelatedResource(this Resource resource, Predicate<Resource> conditionToMatch, Func<Resource, IEnumerable<Resource>> methodToNavigate)
        {
            if (resource is null || conditionToMatch(resource))
                return resource;
            else
            {
                foreach (var subsequentResource in methodToNavigate(resource))
                {
                    var matchingResource = GetFirstRelatedResource(subsequentResource, conditionToMatch, methodToNavigate);
                    if (!(matchingResource is null))
                        return matchingResource;
                }

                return null;
            }
        }
    }
}
