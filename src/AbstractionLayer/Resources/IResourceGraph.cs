using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer.Capabilities;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Component that holds the resource graph and maintains references to all nodes
    /// </summary>
    public interface IResourceGraph
    {
        /// <summary>
        /// Get the resource with this id
        /// </summary>
        Resource Get(long id);

        /// <summary>
        /// Get only resources of this type
        /// </summary>
        TResource GetResource<TResource>()
            where TResource : class, IResource;

        /// <summary>
        /// Get typed resource by id
        /// </summary>
        TResource GetResource<TResource>(long id)
            where TResource : class, IResource;

        /// <summary>
        /// Get typed resource by name
        /// </summary>
        TResource GetResource<TResource>(string name)
            where TResource : class, IResource;

        /// <summary>
        /// Get the only resource that matches the given predicate
        /// </summary>
        /// <returns>Instance if only one match was found, otherwise <value>null</value></returns>
        TResource GetResource<TResource>(Func<TResource, bool> predicate)
            where TResource : class, IResource;

        /// <summary>
        /// Get all resources of this type
        /// </summary>
        IEnumerable<TResource> GetResources<TResource>()
            where TResource : class, IResource;

        /// <summary>
        /// Get all resources of this type that match the predicate
        /// </summary>
        IEnumerable<TResource> GetResources<TResource>(Func<TResource, bool> predicate)
            where TResource : class, IResource;
    }
}