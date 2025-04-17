// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Resource management API used to interact with resources on an abstract level
    /// </summary>
    public interface IResourceManagement
    {
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
        /// Get the only resource that provides the required capabilities.
        /// </summary>
        /// <returns>Instance if only one match was found, otherwise <value>null</value></returns>
        TResource GetResource<TResource>(ICapabilities requiredCapabilities)
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
        /// Get all resources of this type that provide the required capabilities
        /// </summary>
        IEnumerable<TResource> GetResources<TResource>(ICapabilities requiredCapabilities)
            where TResource : class, IResource;

        /// <summary>
        /// Get all resources of this type that match the predicate
        /// </summary>
        IEnumerable<TResource> GetResources<TResource>(Func<TResource, bool> predicate)
            where TResource : class, IResource;

        /// <summary>
        /// Get all resources including the private ones of this type that match the predicate
        /// </summary>
        IEnumerable<TResource> GetAllResources<TResource>(Func<TResource, bool> predicate)
            where TResource : class, IResource;

        /// <summary>
        /// Create and initialize a resource
        /// </summary>
        long Create(Type resourceType, Action<Resource> initializer);

        /// <summary>
        /// Read data from a resource
        /// </summary>
        TResult Read<TResult>(long id, Func<Resource, TResult> accessor);

        /// <summary>
        /// Modify the resource. 
        /// </summary>
        /// <param name="id">Id of the resource</param>
        /// <param name="modifier">Modifier delegate, must return <value>true</value> in order to save changes</param>
        void Modify(long id, Func<Resource, bool> modifier);

        /// <summary>
        /// Create and initialize a resource
        /// </summary>
        bool Delete(long id);

        /// <summary>
        /// Event raised when a resource was added at runtime
        /// </summary>
        event EventHandler<IResource> ResourceAdded;

        /// <summary>
        /// Event raised when a resource was removed at runtime
        /// </summary>
        event EventHandler<IResource> ResourceRemoved;

        /// <summary>
        /// Raised when the capabilities have changed.
        /// </summary>
        event EventHandler<ICapabilities> CapabilitiesChanged;
    }
}
