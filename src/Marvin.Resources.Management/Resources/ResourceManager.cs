using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Marvin.AbstractionLayer.Capabilities;
using Marvin.AbstractionLayer.Resources;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Model;
using Marvin.Modules;
using Marvin.Resources.Model;
using Marvin.Tools;

namespace Marvin.Resources.Management
{
    [Plugin(LifeCycle.Singleton, typeof(IResourceManager))]
    internal class ResourceManager : IResourceManager, IResourceCreator
    {
        #region Dependency Injection

        /// <summary>
        /// Reference to the resource graph
        /// </summary>
        public IManagedResourceGraph Graph { get; set; }

        /// <summary>
        /// Type controller managing the type tree and proxy creation
        /// </summary>
        public IResourceTypeController TypeController { get; set; }

        /// <summary>
        /// Component responsible for linking of the object graph
        /// </summary>
        public IResourceLinker ResourceLinker { get; set; }

        /// <summary>
        /// Access to the database
        /// </summary>
        public IUnitOfWorkFactory UowFactory { get; set; }

        /// <summary>
        /// Error reporting in case a resource crashes
        /// </summary>
        public IModuleErrorReporting ErrorReporting { get; set; }

        /// <summary>
        /// Logger for the ResourceManager
        /// </summary>
        [UseChild(nameof(ResourceManager))]
        public IModuleLogger Logger { get; set; }

        #endregion

        #region Fields

        /// <summary>
        /// Enum of the different startup phases of the resources
        /// </summary>
        private enum ResourceStartupPhase
        {
            /// <summary>
            /// Loading the existing resources or creating a root resource
            /// </summary>
            LoadResources,
            /// <summary>
            /// Calling Initialize() foreach loaded resource
            /// </summary>
            Initializing,
            /// <summary>
            /// Every loaded resource is initialized
            /// </summary>
            Initialized,
            /// <summary>
            /// Calling Start() foreach loaded resource
            /// </summary>
            Starting,
            /// <summary>
            /// Every loaded resource is started.
            /// </summary>
            Started,
            /// <summary>
            /// Calling Stop() foreach resource
            /// </summary>
            Stopping,
            /// <summary>
            /// All resources are stopped
            /// </summary>
            Stopped
        }

        /// <summary>
        /// Current phase of the Resource-Startup-Phase
        /// </summary>
        private ResourceStartupPhase _startup;

        #endregion

        #region LifeCycle

        public void Initialize()
        {
            _startup = ResourceStartupPhase.LoadResources;
            using (var uow = UowFactory.Create(ContextMode.AllOff))
            {
                // Create all objects
                var allResources = ResourceEntityAccessor.FetchResourceTemplates(uow);
                if (allResources.Count > 0)
                {
                    LoadResources(allResources);
                }
                else
                {
                    Logger.LogEntry(LogLevel.Warning, "The ResourceManager initialized without a resource." +
                                                      "Execute a resource initializer to add resources with \"exec ResourceManager initialize\"");
                }
            }

            _startup = ResourceStartupPhase.Initializing;
            // Boot resources
            Parallel.ForEach(Graph.GetAll(), resourceWrapper =>
            {
                try
                {
                    resourceWrapper.Initialize();
                }
                catch (Exception e)
                {
                    resourceWrapper.ErrorOccured();
                    ErrorReporting.ReportWarning(this, e);
                }
            });
            _startup = ResourceStartupPhase.Initialized;
        }

        /// <summary>
        /// Load and link all resources from the databse
        /// </summary>
        private void LoadResources(ICollection<ResourceEntityAccessor> allResources)
        {
            // Create resource objects on multiple threads
            var query = from template in allResources.AsParallel()
                        select template.Instantiate(TypeController, this);
            foreach (var resource in query)
                AddResource(resource, false);

            // Link them to each other
            Parallel.ForEach(allResources, LinkReferences);

            // Register events after all links were set
            foreach (var resourceWrapper in Graph.GetAll())
                RegisterEvents(resourceWrapper.Target);
        }

        /// <summary>
        /// Add resource to all collections and register to the <see cref="Resource.Changed"/> event
        /// </summary>
        private void AddResource(Resource instance, bool registerEvents)
        {
            // Add instance to the graph
            var wrapped = Graph.Add(instance);

            // Register to events
            if (registerEvents)
                RegisterEvents(instance);

            switch (_startup)
            {
                case ResourceStartupPhase.LoadResources:
                    // do nothing
                    break;
                case ResourceStartupPhase.Initializing:
                case ResourceStartupPhase.Initialized:
                    // Resources those are created during the initialize of a resource are automaticaly initialized also.
                    wrapped.Initialize();
                    break;
                case ResourceStartupPhase.Starting:
                case ResourceStartupPhase.Started:
                    // Resources those are created during the start of a resource are automaticaly initialized and started also.
                    wrapped.Initialize();
                    wrapped.Start();
                    break;
                case ResourceStartupPhase.Stopping:
                case ResourceStartupPhase.Stopped:
                    // do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Inform listeners about the new resource
            var publicResource = instance as IPublicResource;
            if (publicResource != null)
                RaiseResourceAdded(publicResource);
        }

        /// <summary>
        /// Register a resources events
        /// </summary>
        private void RegisterEvents(Resource instance)
        {
            instance.Changed += OnResourceChanged;

            var asPublic = instance as IPublicResource;
            if (asPublic != null)
                asPublic.CapabilitiesChanged += RaiseCapabilitiesChanged;

            foreach (var autoSaveCollection in ResourceLinker.GetAutoSaveCollections(instance))
                autoSaveCollection.CollectionChanged += OnAutoSaveCollectionChanged;
        }

        /// <summary>
        /// Register a resources events
        /// </summary>
        private void UnregisterEvents(Resource instance)
        {
            instance.Changed -= OnResourceChanged;

            var asPublic = instance as IPublicResource;
            if (asPublic != null)
                asPublic.CapabilitiesChanged -= RaiseCapabilitiesChanged;

            foreach (var autoSaveCollection in ResourceLinker.GetAutoSaveCollections(instance))
                autoSaveCollection.CollectionChanged -= OnAutoSaveCollectionChanged;
        }

        /// <summary>
        /// Event handler when a resource was modified and the changes need to
        /// written to storage
        /// </summary>
        private void OnResourceChanged(object sender, EventArgs eventArgs)
        {
            Save((Resource)sender);
        }

        /// <summary>
        /// Build object graph from simplified <see cref="ResourceEntityAccessor"/> and flat resource list
        /// </summary>
        private void LinkReferences(ResourceEntityAccessor entityAccessor)
        {
            ResourceLinker.LinkReferences(entityAccessor.Instance, entityAccessor.Relations);
        }

        public void Start()
        {
            _startup = ResourceStartupPhase.Starting;
            Parallel.ForEach(Graph.GetAll(), resourceWrapper =>
            {
                try
                {
                    resourceWrapper.Start();
                }
                catch (Exception e)
                {
                    resourceWrapper.ErrorOccured();
                    ErrorReporting.ReportWarning(this, e);
                }
            });
            _startup = ResourceStartupPhase.Started;
        }

        public void Stop()
        {
            _startup = ResourceStartupPhase.Stopping;

            Parallel.ForEach(Graph.GetAll(), resourceWrapper =>
                {
                    try
                    {
                        resourceWrapper.Stop();
                    }
                    catch (Exception e)
                    {
                        ErrorReporting.ReportWarning(this, e);
                    }
                });

            _startup = ResourceStartupPhase.Stopped;
        }

        public void Dispose()
        {
            foreach (var resourceWrapper in Graph.GetAll())
            {
                UnregisterEvents(resourceWrapper.Target);
            }
        }

        #endregion

        public Resource Get(long id) => Graph.Get(id);

        public Resource Create(string type)
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

        private readonly object _fallbackLock = new object();
        public void Save(Resource resource)
        {
            lock (Graph.Get(resource.Id) ?? _fallbackLock)
            {
                using (var uow = UowFactory.Create())
                {
                    var newResources = new HashSet<Resource>();

                    var entity = ResourceEntityAccessor.SaveToEntity(uow, resource);
                    if (entity.Id == 0)
                        newResources.Add(resource);

                    var newInstances = ResourceLinker.SaveReferences(uow, resource, entity);
                    newResources.AddRange(newInstances);

                    try
                    {
                        uow.Save();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(LogLevel.Error, ex, "Error saving resource {0}-{1}!", resource.Id, resource.Name);
                        throw;
                    }

                    foreach (var instance in newResources)
                        AddResource(instance, true);
                }
            }
        }

        /// <summary>
        /// A collection with "AutoSave = true" was modified. Write current state to the database
        /// </summary>
        private void OnAutoSaveCollectionChanged(object sender, ReferenceCollectionChangedEventArgs args)
        {
            var instance = args.Parent;
            var property = args.CollectionProperty;

            using (var uow = UowFactory.Create())
            {
                var newResources = ResourceLinker.SaveSingleCollection(uow, instance, property);

                uow.Save();

                foreach (var newResource in newResources)
                    AddResource(newResource, true);
            }
        }

        public IReadOnlyList<Resource> GetRoots()
        {
            return Graph.GetRoots();
        }

        public void ExecuteInitializer(IResourceInitializer initializer)
        {
            var roots = initializer.Execute(this);

            if (roots.Count == 0)
                throw new InvalidOperationException("ResourceInitializer must return at least one resource");

            using (var uow = UowFactory.Create())
            {
                ResourceLinker.SaveRoots(uow, roots);
                uow.Save();
            }
        }

        public bool Start(Resource resource)
        {
            try
            {
                ((IPlugin)resource).Start();
                return true;
            }
            catch (Exception e)
            {
                ErrorReporting.ReportWarning(this, e);
                return false;
            }
        }

        public bool Stop(Resource resource)
        {
            try
            {
                ((IPlugin)resource).Stop();
                return true;
            }
            catch (Exception e)
            {
                ErrorReporting.ReportWarning(this, e);
                return false;
            }
        }

        #region IResourceCreator

        public TResource Instantiate<TResource>() where TResource : Resource
        {
            return (TResource)Instantiate(typeof(TResource).ResourceType());
        }

        public TResource Instantiate<TResource>(string type) where TResource : class, IResource
        {
            return Instantiate(type) as TResource;
        }

        public Resource Instantiate(string type)
        {
            return Create(type);
        }

        public bool Destroy(IResource resource)
        {
            return Destroy(resource, false);
        }

        public bool Destroy(IResource resource, bool permanent)
        {
            var instance = (Resource)resource;
            ((IPlugin)resource).Stop();

            // Load entity and relations to disconnect resource and remove from database
            using (var uow = UowFactory.Create())
            {
                var resourceRepository = uow.GetRepository<IResourceEntityRepository>();
                var relationRepository = uow.GetRepository<IResourceRelationRepository>();

                // Fetch entity and relations
                // Update properties on the references and get rid of relation entities
                var entity = resourceRepository.GetByKey(instance.Id);
                var relations = ResourceRelationAccessor.FromEntity(uow, entity);
                foreach (var relation in relations)
                {
                    var reference = Graph.Get(relation.ReferenceId);

                    ResourceLinker.RemoveLinking(resource, reference);

                    if (permanent)
                        relationRepository.Remove(relation.Entity);
                }

                resourceRepository.Remove(entity, permanent);

                uow.Save();
            }

            // Unregister from all events to avoid memory leaks
            UnregisterEvents(instance);

            // Destroy the object
            TypeController.Destroy(instance);

            // Remove from internal collections
            return Graph.Remove(instance);
        }

        #endregion

        #region IResourceManagement

        public TResource GetResource<TResource>() where TResource : class, IPublicResource
        {
            return Graph.GetResource<TResource>();
        }

        public TResource GetResource<TResource>(long id) where TResource : class, IPublicResource
        {
            return Graph.GetResource<TResource>(r => r.Id == id);
        }

        public TResource GetResource<TResource>(string name) where TResource : class, IPublicResource
        {
            return Graph.GetResource<TResource>(r => r.Name == name);
        }

        public TResource GetResource<TResource>(ICapabilities requiredCapabilities) where TResource : class, IPublicResource
        {
            return Graph.GetResource<TResource>(r => requiredCapabilities.ProvidedBy(r.Capabilities));
        }

        public TResource GetResource<TResource>(Func<TResource, bool> predicate)
            where TResource : class, IPublicResource
        {
            return Graph.GetResource(predicate);
        }

        public IEnumerable<TResource> GetResources<TResource>() where TResource : class, IPublicResource
        {
            return Graph.GetResources<TResource>(r => true);
        }

        public IEnumerable<TResource> GetResources<TResource>(ICapabilities requiredCapabilities) where TResource : class, IPublicResource
        {
            return Graph.GetResources<TResource>(r => requiredCapabilities.ProvidedBy(r.Capabilities));
        }

        public IEnumerable<TResource> GetResources<TResource>(Func<TResource, bool> predicate) where TResource : class, IPublicResource
        {
            return Graph.GetResources(predicate);
        }

        private void RaiseResourceAdded(IPublicResource newResource)
        {
            ResourceAdded?.Invoke(this, newResource);
        }
        public event EventHandler<IPublicResource> ResourceAdded;

        ///
        public event EventHandler<ICapabilities> CapabilitiesChanged;

        private void RaiseCapabilitiesChanged(object originalSender, ICapabilities capabilities)
        {
            CapabilitiesChanged?.Invoke(originalSender, capabilities);
        }

        #endregion
    }
}