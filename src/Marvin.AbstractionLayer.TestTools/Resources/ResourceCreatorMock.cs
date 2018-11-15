using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using Marvin.AbstractionLayer.Resources;
using Marvin.TestTools.UnitTest;
using Marvin.Tools;

namespace Marvin.AbstractionLayer.TestTools.Resources
{
    /// <inheritdoc />
    public class ResourceCreatorMock : IResourceCreator
    {
        private IDictionary<string, Type> _typeMap;

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
                throw new InstanceNotFoundException($"Cannot instantiate {type}");
            }

            SetReferenceCollections(instance);

            instance.Logger = new DummyLogger();

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

        private void SetReferenceCollections(Resource instance)
        {
            var resourceType = instance.GetType();
            var properties = (from prop in resourceType.GetProperties()
                let propType = prop.PropertyType
                where propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(IReferences<>)
                select prop).ToList();
            foreach (var property in properties)
            {
                var listType = typeof(ReferenceCollection<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]);
                var list = Activator.CreateInstance(listType);
                property.SetValue(instance, list);
            }
        }
    }
}
