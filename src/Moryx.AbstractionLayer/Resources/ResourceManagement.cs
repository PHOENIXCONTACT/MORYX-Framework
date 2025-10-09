// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.Modules;

namespace Moryx.AbstractionLayer.Resources;

/// <summary>
/// Resource management API used to interact with resources on an abstract level
/// </summary>
public abstract class ResourceManagement : FacadeBase
{
    /// <summary>
    /// Get only resources of this type
    /// </summary>
    public virtual TResource GetResource<TResource>()
        where TResource : class, IResource
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get typed resource by id
    /// </summary>
    public virtual TResource GetResource<TResource>(long id)
        where TResource : class, IResource
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get typed resource by name
    /// </summary>
    public virtual TResource GetResource<TResource>(string name)
        where TResource : class, IResource
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the only resource that provides the required capabilities.
    /// </summary>
    /// <returns>Instance if only one match was found, otherwise <value>null</value></returns>
    public virtual TResource GetResource<TResource>(ICapabilities requiredCapabilities)
        where TResource : class, IResource
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the only resource that matches the given predicate
    /// </summary>
    /// <returns>Instance if only one match was found, otherwise <value>null</value></returns>
    public virtual TResource GetResource<TResource>(Func<TResource, bool> predicate)
        where TResource : class, IResource
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get all resources of this type
    /// </summary>
    public virtual IEnumerable<TResource> GetResources<TResource>()
        where TResource : class, IResource
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get all resources of this type that provide the required capabilities
    /// </summary>
    public virtual IEnumerable<TResource> GetResources<TResource>(ICapabilities requiredCapabilities)
        where TResource : class, IResource
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get all resources of this type that match the predicate
    /// </summary>
    public virtual IEnumerable<TResource> GetResources<TResource>(Func<TResource, bool> predicate)
        where TResource : class, IResource
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get all resources including the private ones of this type that match the predicate
    /// </summary>
    public virtual IEnumerable<TResource> GetAllResources<TResource>(Func<TResource, bool> predicate)
        where TResource : class, IResource
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Create and initialize a resource
    /// </summary>
    public virtual long Create(Type resourceType, Action<Resource> initializer)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Read data from a resource
    /// </summary>
    public virtual TResult Read<TResult>(long id, Func<Resource, TResult> accessor)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Modify the resource.
    /// </summary>
    /// <param name="id">Id of the resource</param>
    /// <param name="modifier">Modifier delegate, must return <value>true</value> in order to save changes</param>
    public virtual void Modify(long id, Func<Resource, bool> modifier)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Create and initialize a resource
    /// </summary>
    public virtual bool Delete(long id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Raises the <see cref="ResourceAdded"/> event
    /// </summary>
    protected virtual void RaiseResourceAdded(object sender, IResource resource)
    {
        ResourceAdded?.Invoke(this, resource);
    }

    /// <summary>
    /// Raises the <see cref="ResourceRemoved"/> event
    /// </summary>
    protected virtual void RaiseResourceRemoved(object sender, IResource resource)
    {
        ResourceRemoved?.Invoke(this, resource);
    }

    /// <summary>
    /// Raises the <see cref="CapabilitiesChanged"/> event
    /// </summary>
    protected virtual void RaiseCapabilitiesChanged(object sender, ICapabilities capabilities)
    {
        CapabilitiesChanged?.Invoke(this, capabilities);
    }

    /// <summary>
    /// Event raised when a resource was added at runtime
    /// </summary>
    public event EventHandler<IResource> ResourceAdded;

    /// <summary>
    /// Event raised when a resource was removed at runtime
    /// </summary>
    public event EventHandler<IResource> ResourceRemoved;

    /// <summary>
    /// Raised when the capabilities have changed.
    /// </summary>
    public event EventHandler<ICapabilities> CapabilitiesChanged;
}
