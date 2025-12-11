// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Model.Repositories;
using Moryx.Modules;
using Moryx.Resources.Management.Model;
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

        /// <summary>
        /// Config of this module
        /// </summary>
        public ModuleConfig Config { get; set; }

        /// <summary>
        /// Factory to create resource initializers
        /// </summary>
        public IResourceInitializerFactory InitializerFactory { get; set; }

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
        /// Semaphore for thread-safe saving of resources.
        /// </summary>
        private readonly SemaphoreSlim _saveLock = new(1, 1);

        /// <summary>
        /// Look-up of resources that failed during init or start and are excluded from certain calls
        /// </summary>
        private readonly List<Resource> _failedResources = [];

        /// <summary>
        /// Configured resource initializers
        /// </summary>
        private IResourceInitializer[] _initializers;

        #endregion

        #region LifeCycle

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            // Set delegates on graph
            Graph.SaveDelegate = SaveAsync;
            Graph.DestroyDelegate = Destroy;

            _startup = ResourceStartupPhase.LoadResources;
            using (var uow = UowFactory.Create())
            {
                // Create all objects
                var allResources = await ResourceEntityAccessor.FetchResourceTemplates(uow);
                if (allResources.Count > 0)
                    await LoadResources(allResources);
            }

            _startup = ResourceStartupPhase.Initializing;

            // initialize resources
            await Parallel.ForEachAsync(Graph.GetAll(), cancellationToken, InitializeResource);

            _startup = ResourceStartupPhase.Initialized;
        }

        /// <summary>
        /// Handles the initialization of the resource
        /// </summary>
        private async ValueTask InitializeResource(Resource resource, CancellationToken cancellationToken)
        {
            try
            {
                await ((IAsyncInitializable)resource).InitializeAsync(cancellationToken);
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
        private async ValueTask StartResource(Resource resource, CancellationToken cancellationToken)
        {
            try
            {
                await ((IAsyncPlugin)resource).StartAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Warning, e, "Failed to start resource {0}-{1}", resource.Id, resource.Name);
            }
        }

        /// <summary>
        /// Load and link all resources from the database
        /// </summary>
        private async Task LoadResources(ICollection<ResourceEntityAccessor> allResources)
        {
            // Create resource objects on multiple threads
            await Parallel.ForEachAsync(allResources, InstantiateResource);

            // Link them to each other
            Parallel.ForEach(allResources, LinkReferences);

            // Register events after all links were set
            foreach (var resource in Graph.GetAll())
                RegisterEvents(resource);
        }

        /// <summary>
        /// Instantiate object from entity based template
        /// </summary>
        private async ValueTask InstantiateResource(ResourceEntityAccessor template, CancellationToken cancellationToken)
        {
            try
            {
                var resource = template.Instantiate(TypeController, Graph);
                await AddResource(resource, false);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Warning, e, "Failed to instantiate resource {0}-{1}", template.Id, template.Name);
            }
        }

        /// <summary>
        /// Add resource to all collections and register to the <see cref="Resource.Changed"/> event
        /// </summary>
        private async Task AddResource(Resource instance, bool registerEvents)
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
                    // Resources those are created during the initialization of a resource are automatically initialized also.
                    await InitializeResource(resource, CancellationToken.None);
                    break;
                case ResourceStartupPhase.Starting:
                case ResourceStartupPhase.Started:
                    // Resources those are created during the start of a resource are automatically initialized and started also.
                    await InitializeAndStart(resource);
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
        private async Task InitializeAndStart(Resource resource)
        {
            await InitializeResource(resource, CancellationToken.None);
            await StartResource(resource, CancellationToken.None);
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
        /// be written to storage
        /// </summary>
        private void OnResourceChanged(object sender, EventArgs eventArgs)
        {
            _ = Task.Run(() => SaveAsync((Resource)sender));
        }

        /// <summary>
        /// Build object graph from simplified <see cref="ResourceEntityAccessor"/> and flat resource list
        /// </summary>
        private void LinkReferences(ResourceEntityAccessor entityAccessor)
        {
            ResourceLinker.LinkReferences(entityAccessor.Instance, entityAccessor.Relations);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            // Create configured resource initializers
            _initializers = (from importerConfig in Config.Initializers
                select InitializerFactory.Create(importerConfig)).ToArray();

            // start resources
            _startup = ResourceStartupPhase.Starting;
            await Parallel.ForEachAsync(Graph.GetAll().Except(_failedResources), cancellationToken, StartResource);
            _startup = ResourceStartupPhase.Started;
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            _startup = ResourceStartupPhase.Stopping;

            await Parallel.ForEachAsync(Graph.GetAll(), cancellationToken, async (resource, cancelToken) =>
            {
                try
                {
                    await ((IAsyncPlugin)resource).StopAsync(cancelToken);
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

        public async Task SaveAsync(Resource resource)
        {
            await _saveLock.WaitAsync();

            try
            {
                var isNew = resource.Id == 0;

                using var uow = UowFactory.Create();
                var newResources = new HashSet<Resource>();

                var saveResult = await ResourceEntityAccessor.SaveToEntity(uow, resource);
                var entity = saveResult;
                if (uow.IsLinked(resource))
                    newResources.Add(resource);

                var references = new Dictionary<Resource, ResourceEntity>();
                var newInstances = await ResourceLinker.SaveReferencesAsync(uow, resource, entity, references);
                newResources.AddRange(newInstances);

                try
                {
                    await uow.SaveChangesAsync();
                    resource.Id = entity.Id;
                    foreach (var instance in newResources)
                    {
                        if (!references.TryGetValue(instance, out var reference))
                            continue;
                        instance.Id = reference.Id;
                    }

                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, ex, "Error saving resource {0}-{1}!", resource.Id, resource.Name);
                    throw;
                }

                foreach (var instance in newResources)
                    await AddResource(instance, true);

                if (!isNew)
                {
                    RaiseResourceChanged(resource);
                }
            }
            finally
            {
                _saveLock.Release();
            }
        }

        /// <summary>
        /// A collection with "AutoSave = true" was modified. Write current state to the database
        /// </summary>
        private void OnAutoSaveCollectionChanged(object sender, ReferenceCollectionChangedEventArgs args)
        {
            var instance = args.Parent;
            var property = args.CollectionProperty;

            _ = Task.Run(async () =>
            {
                await _saveLock.WaitAsync();

                try
                {
                    using var uow = UowFactory.Create();
                    var newResources = await ResourceLinker.SaveSingleCollectionAsync(uow, instance, property);

                    try
                    {
                        await uow.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, ex, "Error saving collection {2} on resource {0}-{1}!", instance.Id, instance.Name, property.Name);
                        throw;
                    }

                    foreach (var newResource in newResources)
                    {
                        await AddResource(newResource, true);
                    }

                    RaiseResourceChanged(instance);
                }
                finally
                {
                    _saveLock.Release();
                }
            });
        }

        public Task<ResourceInitializerResult> ExecuteInitializer(string initializerName, object parameters)
        {
            var initializer = _initializers.First(i => i.Name == initializerName);
            return ExecuteInitializer(initializer, parameters);
        }

        public async Task<ResourceInitializerResult> ExecuteInitializer(IResourceInitializer initializer, object parameters)
        {
            var result = await initializer.ExecuteAsync(Graph, parameters);

            if (result.InitializedResources.Count == 0)
                throw new InvalidOperationException("ResourceInitializer must return at least one resource");

            if (!result.Saved)
            {
                using var uow = UowFactory.Create();
                await ResourceLinker.SaveRootsAsync(uow, result.InitializedResources);
                await uow.SaveChangesAsync();
            }

            return result;
        }

        #region IResourceCreator

        public async Task<bool> Destroy(IResource resource, bool permanent)
        {
            var instance = (Resource)resource;
            await ((IAsyncPlugin)resource).StopAsync();

            // Load entity and relations to disconnect resource and remove from database
            using (var uow = UowFactory.Create())
            {
                var resourceRepository = uow.GetRepository<IResourceRepository>();
                var relationRepository = uow.GetRepository<IResourceRelationRepository>();

                // Fetch entity and relations
                // Update properties on the references and get rid of relation entities
                var entity = await resourceRepository.GetByKeyAsync(instance.Id);
                var relations = await ResourceRelationAccessor.FromEntity(uow, entity);
                foreach (var relation in relations)
                {
                    var reference = Graph.Get(relation.ReferenceId);

                    ResourceLinker.RemoveLinking(resource, reference);

                    if (permanent)
                        relationRepository.Remove(relation.Entity);
                }

                resourceRepository.Remove(entity, permanent);

                await uow.SaveChangesAsync();
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

        #region IResourceManagement

        private void RaiseResourceAdded(IResource newResource)
        {
            ResourceAdded?.Invoke(this, newResource);
        }
        public event EventHandler<IResource> ResourceAdded;

        private void RaiseResourceRemoved(IResource removedResource)
        {
            ResourceRemoved?.Invoke(this, removedResource);
        }
        public event EventHandler<IResource> ResourceRemoved;

        private void RaiseResourceChanged(IResource changedResource)
        {
            ResourceChanged?.Invoke(this, changedResource);
        }
        public event EventHandler<IResource> ResourceChanged;

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
