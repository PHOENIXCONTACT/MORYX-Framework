// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging.Abstractions;
using Moryx.AbstractionLayer.Resources;
using Moryx.Logging;
using Moryx.Tools;

namespace Moryx.AbstractionLayer.TestTools.Resources
{
    /// <inheritdoc />
    public class ResourceGraphMock : IResourceGraph
    {
        private IDictionary<string, Type> _typeMap;

        /// <summary>
        /// The list of resources in this graph
        /// </summary>
        public List<Resource> Graph { get; set; }

        /// <inheritdoc />
        public Resource Get(long id)
        {
            return Graph.FirstOrDefault(r => r.Id == id);
        }

        /// <inheritdoc />
        public TResource GetResource<TResource>() where TResource : class, IResource
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TResource GetResource<TResource>(long id) where TResource : class, IResource
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TResource GetResource<TResource>(string name) where TResource : class, IResource
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TResource GetResource<TResource>(Func<TResource, bool> predicate) where TResource : class, IResource
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IEnumerable<TResource> GetResources<TResource>() where TResource : class, IResource
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IEnumerable<TResource> GetResources<TResource>(Func<TResource, bool> predicate) where TResource : class, IResource
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Resource Instantiate(string type)
        {
            if (_typeMap == null)
            {
                ReflectionTool.TestMode = true;
                _typeMap = ReflectionTool.GetPublicClasses<Resource>().ToDictionary(c => c.FullName, c => c);
                ReflectionTool.TestMode = false;
            }

            var instance = Activator.CreateInstance(_typeMap[type]) as Resource;
            if (instance == null)
            {
                throw new InvalidOperationException($"Cannot instantiate {type}");
            }

            SetReferenceCollections(instance);

            instance.Logger = new ModuleLogger("Dummy", new NullLoggerFactory());

            return instance;
        }

        /// <inheritdoc />
        public TResource Instantiate<TResource>() where TResource : Resource
        {
            return (TResource)Instantiate(typeof(TResource).FullName);
        }

        /// <inheritdoc />
        public TResource Instantiate<TResource>(string type) where TResource : class, IResource
        {
            return Instantiate(type) as TResource;
        }

        /// <inheritdoc />
        public bool Destroy(IResource resource)
        {
            (resource as IDisposable)?.Dispose();
            return true;
        }

        /// <inheritdoc />
        public bool Destroy(IResource resource, bool permanent)
        {
            return Destroy(resource);
        }

        /// <inheritdoc />
        public void Save(IResource resource)
        {
        }

        private void SetReferenceCollections(Resource instance)
        {
            var resourceType = instance.GetType();
            var properties = (from prop in resourceType.GetProperties()
                              let propType = prop.PropertyType
                              where propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(IReferences<>)
                              select prop).ToList();
            foreach (var property in properties)
            {
                var listType = typeof(ReferenceCollectionMock<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]);
                var list = Activator.CreateInstance(listType);
                property.SetValue(instance, list);
            }
        }
    }
}
