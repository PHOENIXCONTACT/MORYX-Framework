using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marvin.AbstractionLayer.Resources;
using Marvin.Container;
using Marvin.Tools;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Class to build resource proxies at runtime. It also contains a cache for types and instances
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IResourceTypeController), typeof(IResourceTypeTree))]
    internal class ResourceTypeController : IResourceTypeController, IResourceTypeTree
    {
        /// <summary>
        /// Container to read the type structure
        /// </summary>
        public IContainer Container { get; set; }

        /// <summary>
        /// Resource factory to instantiate registred types
        /// </summary>
        public IResourceFactory ResourceFactory { get; set; }

        /// <summary>
        /// Dependency on the linker to provide all reference collections
        /// on the newly created instances.
        /// </summary>
        public IResourceLinker ResourceLinker { get; set; }

        /// <summary>
        /// Component responsible for proxy creation
        /// </summary>
        public ResourceProxyBuilder ProxyBuilder { get; set; }

        /// <summary>
        /// Cache of resource type strings and their proxy types. Some type keys may reference the same
        /// proxy because they do not define any additional public API and can use the same proxy. This
        /// cache is only built on the first module start and kept after a restart to avoid redundant proxy building
        /// </summary>
        private readonly Dictionary<string, string> _proxyTypeCache = new Dictionary<string, string>();

        /// <summary>
        /// Cache of all proxy instances that were created during the runtime of the ResourceManagement. They
        /// all need to be 
        /// </summary>
        private readonly Dictionary<long, IResourceProxy> _proxyCache = new Dictionary<long, IResourceProxy>();

        /// <summary>
        /// All types derived from <see cref="Resource"/>
        /// </summary>
        private readonly List<ResourceTypeNode> _rootTypes = new List<ResourceTypeNode>();

        /// <summary>
        /// Cache to directly access a resource type
        /// </summary>
        private readonly Dictionary<string, ResourceTypeNode> _typeCache = new Dictionary<string, ResourceTypeNode>();

        /// <inheritdoc />
        public IEnumerable<ResourceTypeNode> RootTypes => _rootTypes.AsReadOnly();

        /// <inheritdoc />
        IEnumerable<IResourceTypeNode> IResourceTypeTree.RootTypes => _rootTypes.AsReadOnly();

        /// <inheritdoc />
        public void Start()
        {
            // Define module on first start
            ProxyBuilder.PrepareBuilder();

            BuildTypeTree();
        }

        /// <inheritdoc />
        public void Stop()
        {
            foreach (var proxy in _proxyCache.Values)
            {
                proxy.Detach();
            }
            _proxyCache.Clear();
        }

        /// <summary>
        /// Build the entire type tree and type cache
        /// </summary>
        private void BuildTypeTree()
        {
            // Start with all public types
            var allTypes = new List<Type>();
            var registeredTypes = Container.GetRegisteredImplementations(typeof(IResource)).ToList();
            // Load full type tree from registered resources
            foreach (var type in registeredTypes.Union(ReflectionTool.GetPublicClasses<Resource>()))
            {
                var buffer = type;
                // Exclude the different base types from "Marvin.Resources" from the type tree
                while (buffer != null && !buffer.IsAssignableFrom(typeof(PublicResource)))
                {
                    if (buffer.IsGenericType) // Generic base types appear multiple times, use only the generic type
                        buffer = buffer.GetGenericTypeDefinition();
                    if (!allTypes.Contains(buffer))
                        allTypes.Add(buffer);
                    buffer = buffer.BaseType;
                }
            }

            // Convert tree to TypeLinker objects
            foreach (var type in allTypes.Where(t => t.BaseType.IsAssignableFrom(typeof(PublicResource))))
            {
                var linker = Convert(type, allTypes, registeredTypes);
                _rootTypes.Add(linker);
            }
        }

        private ResourceTypeNode Convert(Type type, ICollection<Type> allTypes, ICollection<Type> registeredTypes, ResourceTypeNode baseType = null)
        {
            // Create linker from type
            var linker = new ResourceTypeNode
            {
                ResourceType = type,
                BaseType = baseType,
                IsRegistered = registeredTypes.Contains(type),
                Constructors = (from method in type.GetMethods()
                                let att = method.GetCustomAttribute<ResourceConstructorAttribute>()
                                where att != null
                                orderby att.IsDefault descending
                                select method).ToArray()
            };

            // Find all derived types
            var derived = allTypes.Where(t => (t.BaseType.IsGenericType ? t.BaseType.GetGenericTypeDefinition() : t.BaseType) == type);
            linker.DerivedTypes = derived.Select(t => Convert(t, allTypes, registeredTypes, linker)).ToArray();

            // Save reference in type cache
            _typeCache[type.Name] = linker;

            return linker;
        }

        public Resource Create(string type)
        {
            var linker = _typeCache[type];
            var instance = linker.IsRegistered
                ? (Resource)ResourceFactory.Create(type) // Create with factory
                : (Resource)Activator.CreateInstance(linker.ResourceType); // Create manually

            // Set reference collections
            ResourceLinker.SetReferenceCollections(instance);

            return instance;
        }

        public void Destroy(Resource instance)
        {
            var linker = _typeCache[instance.GetType().Name];
            // Only factory instances need to be destroyed
            if (linker.IsRegistered)
                ResourceFactory.Destroy(instance);
            else
                ((IDisposable)instance).Dispose();

            // If there is currently a proxy for this resource detach and destroy it
            var id = instance.Id;
            if (_proxyCache.ContainsKey(id))
            {
                _proxyCache[id].Detach();
                _proxyCache.Remove(id);
            }
        }

        /// <inheritdoc />
        public IEnumerable<ResourceTypeNode> SupportedTypes(Type constraint)
        {
            return SupportedTypes(new[] { constraint });
        }
        /// <inheritdoc />
        IEnumerable<IResourceTypeNode> IResourceTypeTree.SupportedTypes(Type constraint) => SupportedTypes(constraint);

        /// <inheritdoc />
        public IEnumerable<ResourceTypeNode> SupportedTypes(ICollection<Type> constraints)
        {
            var types = new List<ResourceTypeNode>();
            foreach (var typeLinker in RootTypes)
            {
                SupportingType(typeLinker, constraints, types);
            }
            return types;
        }
        /// <inheritdoc />
        IEnumerable<IResourceTypeNode> IResourceTypeTree.SupportedTypes(ICollection<Type> constraints) => SupportedTypes(constraints);


        /// <summary>
        /// Recursively check if any type in the tree supports the referenced type
        /// </summary>
        /// <param name="type">Root type from <see cref="IResourceTypeController.RootTypes"/></param>
        /// <param name="typeConstraints">All type constraints a candidate needs to match to be compatible</param>
        /// <param name="supportedTypes">Current list of supported types</param>
        /// <returns></returns>
        private static void SupportingType(ResourceTypeNode type, ICollection<Type> typeConstraints, ICollection<ResourceTypeNode> supportedTypes)
        {
            // This type supports the property -> Convert it and break recursion
            if (typeConstraints.All(tc => tc.IsAssignableFrom(type.ResourceType)))
            {
                supportedTypes.Add(type);
                return;
            }

            // Otherwise check its derived types
            foreach (var derivedType in type.DerivedTypes)
            {
                SupportingType(derivedType, typeConstraints, supportedTypes);
            }
        }

        public IResource GetProxy(Resource instance)
        {
            lock (_proxyCache)
            {
                lock (_proxyTypeCache)
                {
                    return GetOrCreateProxy(instance);
                }
            }
        }

        /// <summary>
        /// Thread safe implementation called from <see cref="GetProxy"/>
        /// </summary>
        private IResource GetOrCreateProxy(Resource instance)
        {
            // Did we build a proxy for this instance before?
            if (_proxyCache.ContainsKey(instance.Id))
                return _proxyCache[instance.Id];

            var resourceType = instance.GetType();
            // Did we build a proxy type before, but for a different instance?
            if (_proxyTypeCache.ContainsKey(resourceType.Name))
                return _proxyCache[instance.Id] = InstantiateProxy(resourceType.Name, instance);

            // Build the proxy type for this resource type
            ProvideProxyType(resourceType);
            return _proxyCache[instance.Id] = InstantiateProxy(resourceType.Name, instance);
        }

        /// <summary>
        /// Instantiate proxy object for a given resource type name
        /// </summary>
        private IResourceProxy InstantiateProxy(string typeName, Resource instance)
        {
            var proxyType = ProxyBuilder.GetType(_proxyTypeCache[typeName]);
            var proxyInstance = (IResourceProxy)Activator.CreateInstance(proxyType, instance, this);
            proxyInstance.Attach();
            return proxyInstance;
        }

        /// <summary>
        /// Make sure the <see cref="_proxyTypeCache"/> contains an entry for the given type
        /// </summary>
        private void ProvideProxyType(Type resourceType)
        {
            // Step 1: Find the least specific base type that offers the same amount of interfaces
            var targetType = _typeCache[resourceType.Name];
            var linker = targetType;

            var interfaces = RelevantInterfaces(linker);
            // Move up the type tree until the parent offers less interfaces than the current linker
            while (linker.BaseType != null && interfaces.Count == RelevantInterfaces(linker.BaseType).Count)
            {
                linker = linker.BaseType;
            }

            // Step 2: Check if we already created a proxy for this type. If we already
            // did use this one for the requested type as well.
            if (_proxyTypeCache.ContainsKey(linker.Name))
            {
                _proxyTypeCache[targetType.Name] = _proxyTypeCache[linker.Name];
                return;
            }

            // Step 3: Build a proxy type for the least specific base type
            var proxyType = ProxyBuilder.Build(linker.ResourceType, interfaces);

            // Step 4: Assign the new proxy type to all derived types from the
            // match to the originally requested one
            _proxyTypeCache[linker.Name] = proxyType.Name;
            while (targetType != null && targetType != linker)
            {
                _proxyTypeCache[targetType.Name] = proxyType.Name;
                targetType = targetType.BaseType;
            }
        }

        /// <summary>
        /// Get all interfaces of a linker that are relevant for the public proxy. This excludes all non-public
        /// interfaces or interfaces that are not derived from IPublicResource. 
        /// </summary>
        private static IReadOnlyList<Type> RelevantInterfaces(ResourceTypeNode node)
        {
            var interfaces = node.ResourceType.GetInterfaces();
            var relevantInterfaces = new List<Type>(interfaces.Length); // At max all interfaces are relevant

            // Add all resources derived from IResource, but not IResource itself
            relevantInterfaces.AddRange(from resourceInterface in interfaces
                                        where resourceInterface.IsPublic
                                              && typeof(IResource).IsAssignableFrom(resourceInterface)
                                              && !resourceInterface.IsAssignableFrom(typeof(IResource))
                                        select resourceInterface);

            // Add all interfaces that are NOT derived from IResource BUT part of any of the relevant interfaces
            relevantInterfaces.AddRange(from generalInterface in interfaces
                                        where !generalInterface.IsAssignableFrom(typeof(IResource)) // It should not be a base type if IResource
                                           && !relevantInterfaces.Contains(generalInterface) // It should not be part of the relevant interfaces yet
                                           && relevantInterfaces.Any(generalInterface.IsAssignableFrom) // It is a base type of a relevant interface
                                        select generalInterface);

            return relevantInterfaces;
        }
    }
}