// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
        /// <remarks>
        /// This method returns the actual resource instances, not wrapped by proxy.
        /// As a result, all internal members, including properties and
        /// methods not exposed through interfaces, are accessible.
        ///
        /// This API is intended primarily for endpoint controllers that must export or
        /// inspect the full internal state of a resource.
        ///
        /// Because the returned objects are the originals, the API consumer is responsible for keeping and watching the life-cycle.
        /// Use with extreme caution. Do not keep the instances in memory for later usage.
        /// </remarks>
        IEnumerable<TResource> GetResourcesUnsafe<TResource>(Func<TResource, bool> predicate)
            where TResource : class, IResource;

        /// <summary>
        /// Create and initialize a resource
        /// </summary>
        /// <remarks>
        /// The <param name="initializer"></param> action uses the actual resource instance, not wrapped by proxy.
        /// As a result, all internal members, including properties and
        /// methods not exposed through interfaces, are accessible.
        ///
        /// This API is intended primarily for endpoint controllers that must export or
        /// inspect the full internal state of a resource.
        ///
        /// Because the returned objects are the originals, the API consumer is responsible for keeping and watching the life-cycle.
        /// Use with extreme caution. Do not keep the instance in memory for later usage.
        /// </remarks>
        long CreateUnsafe(Type resourceType, Action<Resource> initializer);

        /// <summary>
        /// Read data from a resource
        /// </summary>
        /// <remarks>
        /// The <param name="accessor"></param> action uses the actual resource instance, not wrapped by proxy.
        /// As a result, all internal members, including properties and
        /// methods not exposed through interfaces, are accessible.
        ///
        /// This API is intended primarily for endpoint controllers that must export or
        /// inspect the full internal state of a resource.
        ///
        /// Because the returned objects are the originals, the API consumer is responsible for keeping and watching the life-cycle.
        /// Use with extreme caution. Do not keep the instance in memory for later usage.
        /// </remarks>
        TResult ReadUnsafe<TResult>(long id, Func<Resource, TResult> accessor);

        /// <summary>
        /// Modify the resource.
        /// </summary>
        /// <param name="id">Id of the resource</param>
        /// <param name="modifier">Modifier delegate, must return <value>true</value> in order to save changes</param>
        /// <remarks>
        /// The <param name="modifier"></param> action uses the actual resource instance, not wrapped by proxy.
        /// As a result, all internal members, including properties and
        /// methods not exposed through interfaces, are accessible.
        ///
        /// This API is intended primarily for endpoint controllers that must export or
        /// inspect the full internal state of a resource.
        ///
        /// Because the returned objects are the originals, the API consumer is responsible for keeping and watching the life-cycle.
        /// Use with extreme caution. Do not keep the instance in memory for later usage.
        /// </remarks>
        void ModifyUnsafe(long id, Func<Resource, bool> modifier);

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
