// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Reflection;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;

namespace Moryx.Resources.Management
{
    /// <summary>
    /// Resource grap implementation
    /// In AL 3 this remains internal to avoid conflicts with other APIs
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IResourceGraph), typeof(IManagedResourceGraph))]
    internal class ResourceGraph : IManagedResourceGraph
    {
        /// <summary>
        /// Type controller to create new resource objects
        /// </summary>
        public IResourceTypeController TypeController { get; set; }

        private readonly ReaderWriterLockSlim _graphLock = new();

        /// <summary>
        /// All resources of the graph
        /// </summary>
        private readonly IDictionary<long, Resource> _graph = new Dictionary<long, Resource>();

        public Action<Resource> SaveDelegate { get; set; }

        public Func<Resource, bool, bool> DestroyDelegate { get; set; }

        public Resource Add(Resource instance)
        {
            _graphLock.EnterWriteLock();
            var wrapper = _graph[instance.Id] = instance;
            _graphLock.ExitWriteLock();

            return wrapper;
        }

        public bool Remove(Resource instance)
        {
            _graphLock.EnterWriteLock();
            var found = _graph.Remove(instance.Id);
            _graphLock.ExitWriteLock();

            return found;
        }

        public Resource Get(long id)
        {
            return GetResource(id);
        }

        public Resource GetResource(long id)
        {
            _graphLock.EnterReadLock();
            var match = _graph.ContainsKey(id) ? _graph[id] : null;
            _graphLock.ExitReadLock();

            return match;
        }

        public ICollection<Resource> GetAll()
        {
            _graphLock.EnterReadLock();
            var values = _graph.Values;
            _graphLock.ExitReadLock();
            return values;
        }

        public TResource GetResource<TResource>() where TResource : class, IResource
        {
            return GetResource<TResource>(r => true);
        }

        public TResource GetResource<TResource>(long id) where TResource : class, IResource
        {
            return Get(id) as TResource;
        }

        public TResource GetResource<TResource>(string name) where TResource : class, IResource
        {
            return GetResource<TResource>(r => r.Name == name);
        }

        public TResource GetResource<TResource>(Func<TResource, bool> predicate)
            where TResource : class, IResource
        {
            // Return a single resource
            var match = GetResources(predicate).SingleOrDefault();
            if (match == null)
                throw new ResourceNotFoundException();

            return match;
        }

        public IEnumerable<TResource> GetResources<TResource>() where TResource : class, IResource
        {
            return GetResources<TResource>(r => true);
        }

        public IEnumerable<TResource> GetResources<TResource>(Func<TResource, bool> predicate) where TResource : class, IResource
        {
            IEnumerable<TResource> matches;
            _graphLock.EnterReadLock();
            matches = from resource in _graph.Values
                      let target = resource as TResource
                      where target != null && predicate(target)
                      select target;
            _graphLock.ExitReadLock();

            return matches;
        }

        public Resource Instantiate(string type)
        {
            // Create simplified template and instantiate
            var template = new ResourceEntityAccessor { Type = type };
            var instance = template.Instantiate(TypeController, this);

            // Initially set name to value of DisplayNameAttribute if available
            var typeObj = instance.GetType();
            var displayNameAttr = typeObj.GetCustomAttribute<DisplayNameAttribute>();
            instance.Name = displayNameAttr?.DisplayName ?? typeObj.Name;

            return instance;
        }

        public TResource Instantiate<TResource>() where TResource : Resource
        {
            return (TResource)Instantiate(typeof(TResource).ResourceType());
        }

        public TResource Instantiate<TResource>(string type) where TResource : class, IResource
        {
            return Instantiate(type) as TResource;
        }

        public void Save(IResource resource)
        {
            SaveDelegate((Resource)resource);
        }

        public bool Destroy(IResource resource)
        {
            return Destroy(resource, false);
        }

        public bool Destroy(IResource resource, bool permanent)
        {
            return DestroyDelegate((Resource)resource, permanent);
        }
    }
}
