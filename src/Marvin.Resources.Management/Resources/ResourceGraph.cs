using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer.Capabilities;
using Marvin.AbstractionLayer.Resources;
using Marvin.Container;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Resource grap implementation
    /// In AL 3 this remains internal to avoid conflicts with other APIs
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IResourceGraph), typeof(IManagedResourceGraph))]
    internal class ResourceGraph : IManagedResourceGraph
    {
        /// <summary>
        /// Type controller
        /// </summary>
        public IResourceTypeController TypeController { get; set; }

        /// <summary>
        /// All resources of the graph
        /// TODO: Consider replacing with standard dictionary and self-implement sync for improved performance
        /// </summary>
        private readonly IDictionary<long, ResourceWrapper> _graph = new ConcurrentDictionary<long, ResourceWrapper>();

        /// <summary>
        /// Quick access to all public resources
        /// </summary>
        private readonly ICollection<IPublicResource> _publicResources = new SynchronizedCollection<IPublicResource>();

        
        public ResourceWrapper Add(Resource instance)
        {
            var wrapper = _graph[instance.Id] = new ResourceWrapper(instance);

            var publicResource = instance as IPublicResource;
            if (publicResource != null)
                _publicResources.Add(publicResource);

            return wrapper;
        }

        
        public bool Remove(Resource instance)
        {
            if (_graph.Remove(instance.Id))
            {
                _publicResources.Remove(instance as IPublicResource);
                return true;
            }
            return false;
        }

        public Resource Get(long id)
        {
            return _graph.ContainsKey(id) ? _graph[id].Target : null;
        }

        public ResourceWrapper GetWrapper(long id)
        {
            return _graph.ContainsKey(id) ? _graph[id] : null;
        }

        public ICollection<ResourceWrapper> GetAll()
        {
            return _graph.Values;
        }

        public IReadOnlyList<Resource> GetRoots()
        {
            return _graph.Values.Select(wrapper => wrapper.Target).Where(resource => resource.Parent == null).ToList();
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
            // Use short cut if a public resource is requested
            if (typeof(IPublicResource).IsAssignableFrom(typeof(TResource)))
                return _publicResources.Where(p => p.Capabilities != NullCapabilities.Instance && _graph[p.Id].State.IsAvailable).OfType<TResource>().Where(predicate);

            // Otherwise iterate the full graph
            return (from wrapper in _graph.Values
                    let target = wrapper.Target as TResource
                    where target != null && predicate(target)
                    select target);
        }
    }
}