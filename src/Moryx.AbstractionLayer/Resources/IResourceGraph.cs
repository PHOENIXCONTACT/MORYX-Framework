// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources
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

        /// <summary>
        /// Create a new resource instance but DO NOT save it
        /// </summary>
        Resource Instantiate(string type);

        /// <summary>
        /// Instantiate a typed resource
        /// </summary>
        TResource Instantiate<TResource>()
            where TResource : class, IResource;

        /// <summary>
        /// Instantiate a typed resource
        /// </summary>
        TResource Instantiate<TResource>(string type)
            where TResource : class, IResource;

        /// <summary>
        /// Write changes on this object to the database
        /// </summary>
        Task SaveAsync(IResource resource);

        /// <summary>
        /// Remove resource, but only flag it deleted
        /// </summary>
        Task<bool> DestroyAsync(IResource resource);

        /// <summary>
        /// Remove a resource permanently and irreversible
        /// </summary>
        Task<bool> DestroyAsync(IResource resource, bool permanent);
    }
}
