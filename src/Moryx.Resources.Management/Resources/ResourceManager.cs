// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Model.Repositories;
using Moryx.Modules;
using Moryx.Resources.Model;
using Moryx.Tools;
using IResource = Moryx.AbstractionLayer.Resources.IResource;

namespace Moryx.Resources.Management
{
    [Plugin(LifeCycle.Singleton, typeof(IResourceManager))]
    internal class ResourceManager : IResourceManager
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
        public IUnitOfWorkFactory<ResourcesContext> UowFactory { get; set; }

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

        /// <summary>
        /// Fallback lock object if a new instance is saved BEFORE having the wrapper as a lock object
        /// </summary>
        private readonly object _fallbackLock = new();

        /// <summary>
        /// Look-up of resources that failed during init or start and are excluded from certain calls
        /// </summary>
        private List<Resource> _failedResources = [];

        #endregion

        #region LifeCycle

        public void Initialize()
        {
            // Set delegates on graph
            Graph.SaveDelegate = Save;
            Graph.DestroyDelegate = Destroy;

            _startup = ResourceStartupPhase.LoadResources;
            using (var uow = UowFactory.Create())
            {
                // Create all objects
                var allResources = ResourceEntityAccessor.FetchResourceTemplates(uow);
                if (allResources.Count > 0)
                    LoadResources(allResources);
            }

            _startup = ResourceStartupPhase.Initializing;
            // initialize resources
            Parallel.ForEach(Graph.GetAll(), InitializeResource);
            _startup = ResourceStartupPhase.Initialized;
        }

        /// <summary>
        /// Handles the initialization of the resource
        /// </summary>
        /// <param name="resource"></param>
        private void InitializeResource(Resource resource)
        {
            try
            {
                ((IInitializable)resource).Initialize();
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Warning, e, "Failed to initialize resource {0}-{1}", resource.Id, resource.Name);
                // Track resources as failed to exclude from future calls
                lock (_failedResources)
                    _failedResources.Add(resource);
            }
        }

        /// <summary>
        /// Starts a resource and handles in case of failure.
        /// </summary>
        /// <param name="resource"></param>
        private void StartResource(Resource resource)
        {
            try
            {
                ((IPlugin)resource).Start();
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Warning, e, "Failed to start resource {0}-{1}", resource.Id, resource.Name);
            }
        }

        /// <summary>
        /// Load and link all resources from the database
        /// </summary>
        private void LoadResources(ICollection<ResourceEntityAccessor> allResources)
        {
            // Create resource objects on multiple threads
            Parallel.ForEach(allResources, InstantiateResource);

            // Link them to each other
            Parallel.ForEach(allResources, LinkReferences);

            // Register events after all links were set
            foreach (var resource in Graph.GetAll())
                RegisterEvents(resource);
        }

        /// <summary>
        /// Instantiate object from entity based template
        /// </summary>
        private void InstantiateResource(ResourceEntityAccessor template)
        {
            try
            {
                var resource = template.Instantiate(TypeController, Graph);
                AddResource(resource, false);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Warning, e, "Failed to instantiate resource {0}-{1}", template.Id, template.Name);
            }
        }

        /// <summary>
        /// Add resource to all collections and register to the <see cref="Resource.Changed"/> event
        /// </summary>
        private void AddResource(Resource instance, bool registerEvents)
        {
            // Add instance to the graph
            var resource = Graph.Add(instance);

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
                    // Resources those are created during the initialize of a resource are automatically initialized also.
                    InitializeResource(resource);
                    break;
                case ResourceStartupPhase.Starting:
                case ResourceStartupPhase.Started:
                    // Resources those are created during the start of a resource are automatically initialized and started also.
                    InitializeAndStart(resource);
                    break;
                case ResourceStartupPhase.Stopping:
                case ResourceStartupPhase.Stopped:
                    // do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RaiseResourceAdded(resource);
        }

        /// <summary>
        /// Initialize and start the current resource
        /// </summary>
        /// <param name="resource">Current resource</param>
        private void InitializeAndStart(Resource resource)
        {
            InitializeResource(resource);
            StartResource(resource);
        }

        /// <summary>
        /// Register a resources events
        /// </summary>
        private void RegisterEvents(Resource instance)
        {
            instance.Changed += OnResourceChanged;
            instance.CapabilitiesChanged += RaiseCapabilitiesChanged;

            foreach (var autoSaveCollection in ResourceReferenceTools.GetAutoSaveCollections(instance))
                autoSaveCollection.CollectionChanged += OnAutoSaveCollectionChanged;
        }

        /// <summary>
        /// Register a resources events
        /// </summary>
        private void UnregisterEvents(Resource instance)
        {
            instance.Changed -= OnResourceChanged;
            instance.CapabilitiesChanged -= RaiseCapabilitiesChanged;

            foreach (var autoSaveCollection in ResourceReferenceTools.GetAutoSaveCollections(instance))
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
            // start resources
            _startup = ResourceStartupPhase.Starting;
            Parallel.ForEach(Graph.GetAll().Except(_failedResources), StartResource);
            _startup = ResourceStartupPhase.Started;
        }

        public void Stop()
        {
            _startup = ResourceStartupPhase.Stopping;

            Parallel.ForEach(Graph.GetAll(), resource =>
            {
                try
                {
                    ((IPlugin)resource).Stop();
                    UnregisterEvents(resource);
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Warning, e, "Failed to stop resource {0}-{1}", resource.Id, resource.Name);
                }
            });

            _startup = ResourceStartupPhase.Stopped;
        }

        #endregion

        public void Save(Resource resource)
        {
            lock (Graph.GetResource(resource.Id) ?? _fallbackLock)
            {
                using var uow = UowFactory.Create();
                var newResources = new HashSet<Resource>();

                var saveResult = ResourceEntityAccessor.SaveToEntity(uow, resource);
                var entity = saveResult;
                if (uow.IsLinked(resource))
                    newResources.Add(resource);

                var references = new Dictionary<Resource, ResourceEntity>();
                var newInstances = ResourceLinker.SaveReferences(uow, resource, entity, references);
                newResources.AddRange(newInstances);

                try
                {
                    uow.SaveChanges();
                    resource.Id = entity.Id;
                    foreach (var instance in newResources)
                    {
                        if (!references.ContainsKey(instance))
                            continue;
                        instance.Id = references[instance].Id;
                    }

                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, ex, "Error saving resource {0}-{1}!", resource.Id, resource.Name);
                    throw;
                }

                foreach (var instance in newResources)
                    AddResource(instance, true);
            }
        }

        /// <summary>
        /// A collection with "AutoSave = true" was modified. Write current state to the database
        /// </summary>
        private void OnAutoSaveCollectionChanged(object sender, ReferenceCollectionChangedEventArgs args)
        {
            var instance = args.Parent;
            var property = args.CollectionProperty;

            lock (Graph.GetResource(instance.Id)) // Unlike Save AutoSave collections are ALWAYS part of the Graph
            {
                using var uow = UowFactory.Create();
                var newResources = ResourceLinker.SaveSingleCollection(uow, instance, property);

                try
                {
                    uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, ex, "Error saving collection {2} on resource {0}-{1}!", instance.Id, instance.Name, property.Name);
                    throw;
                }

                foreach (var newResource in newResources)
                    AddResource(newResource, true);
            }
        }

        public void ExecuteInitializer(IResourceInitializer initializer)
        {
            var roots = initializer.Execute(Graph);

            if (roots.Count == 0)
                throw new InvalidOperationException("ResourceInitializer must return at least one resource");

            using var uow = UowFactory.Create();
            ResourceLinker.SaveRoots(uow, roots);
            uow.SaveChanges();
        }

        #region IResourceCreator

        public bool Destroy(IResource resource, bool permanent)
        {
            var instance = (Resource)resource;
            ((IPlugin)resource).Stop();

            // Load entity and relations to disconnect resource and remove from database
            using (var uow = UowFactory.Create())
            {
                var resourceRepository = uow.GetRepository<IResourceRepository>();
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

                uow.SaveChanges();
            }

            // Unregister from all events to avoid memory leaks
            UnregisterEvents(instance);

            // Remove from internal collections
            var removed = Graph.Remove(instance);

            // Notify listeners about the removal of the resource
            if (removed)
                RaiseResourceRemoved(instance);

            // Destroy the object
            TypeController.Destroy(instance);

            return removed;
        }

        #endregion

        #region ResourceManagement

        private void RaiseResourceAdded(IResource newResource)
        {
            ResourceAdded?.Invoke(this, newResource);
        }
        public event EventHandler<IResource> ResourceAdded;

        private void RaiseResourceRemoved(IResource newResource)
        {
            ResourceRemoved?.Invoke(this, newResource);
        }
        public event EventHandler<IResource> ResourceRemoved;

        ///
        public event EventHandler<ICapabilities> CapabilitiesChanged;

        private void RaiseCapabilitiesChanged(object originalSender, ICapabilities capabilities)
        {
            if (Graph.GetAll().Any(x => x.Id == ((IResource)originalSender).Id))
                CapabilitiesChanged?.Invoke(originalSender, capabilities);
        }

        #endregion
    }
}
