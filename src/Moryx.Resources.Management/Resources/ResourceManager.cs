// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Model.Repositories;
using Moryx.Modules;
using Moryx.Resources.Model;
using Moryx.Tools;

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
                {
                    LoadResources(allResources);
                }
                else
                {
                    Logger.Log(LogLevel.Warning, "The ResourceManager initialized without a resource." +
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
                    Logger.Log(LogLevel.Warning, e, "Failed to initialize resource {0}-{1}", resourceWrapper.Target.Id, resourceWrapper.Target.Name);
                }
            });
            _startup = ResourceStartupPhase.Initialized;
        }

        /// <summary>
        /// Load and link all resources from the database
        /// </summary>
        private void LoadResources(ICollection<ResourceEntityAccessor> allResources)
        {
            // Create resource objects on multiple threads
            var query = from template in allResources.AsParallel()
                        select template.Instantiate(TypeController, Graph);
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
                    // Resources those are created during the initialize of a resource are automatically initialized also.
                    wrapped.Initialize();
                    break;
                case ResourceStartupPhase.Starting:
                case ResourceStartupPhase.Started:
                    // Resources those are created during the start of a resource are automatically initialized and started also.
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
            if (instance is IPublicResource publicResource)
                RaiseResourceAdded(publicResource);
        }

        /// <summary>
        /// Register a resources events
        /// </summary>
        private void RegisterEvents(Resource instance)
        {
            instance.Changed += OnResourceChanged;

            if (instance is IPublicResource asPublic)
                asPublic.CapabilitiesChanged += RaiseCapabilitiesChanged;

            foreach (var autoSaveCollection in ResourceReferenceTools.GetAutoSaveCollections(instance))
                autoSaveCollection.CollectionChanged += OnAutoSaveCollectionChanged;
        }

        /// <summary>
        /// Register a resources events
        /// </summary>
        private void UnregisterEvents(Resource instance)
        {
            instance.Changed -= OnResourceChanged;

            if (instance is IPublicResource asPublic)
                asPublic.CapabilitiesChanged -= RaiseCapabilitiesChanged;

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
                    Logger.Log(LogLevel.Warning, e, "Failed to start resource {0}-{1}", resourceWrapper.Target.Id, resourceWrapper.Target.Name);
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
                    UnregisterEvents(resourceWrapper.Target);
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Warning, e, "Failed to stop resource {0}-{1}", resourceWrapper.Target.Id, resourceWrapper.Target.Name);
                }
            });

            _startup = ResourceStartupPhase.Stopped;
        }

        #endregion

        public void Save(Resource resource)
        {
            lock (Graph.GetWrapper(resource.Id) ?? _fallbackLock)
            {
                using var uow = UowFactory.Create();
                var newResources = new HashSet<Resource>();

                var entity = ResourceEntityAccessor.SaveToEntity(uow, resource);
                if (entity.Id == 0)
                    newResources.Add(resource);

                var newInstances = ResourceLinker.SaveReferences(uow, resource, entity);
                newResources.AddRange(newInstances);

                try
                {
                    uow.SaveChanges();
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

            lock (Graph.GetWrapper(instance.Id)) // Unlike Save AutoSave collections are ALWAYS part of the Graph
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
            if (removed && instance is IPublicResource publicResource)
                RaiseResourceRemoved(publicResource);

            // Destroy the object
            TypeController.Destroy(instance);

            return removed;
        }

        #endregion

        #region IResourceManagement


        private void RaiseResourceAdded(IPublicResource newResource)
        {
            ResourceAdded?.Invoke(this, newResource);
        }
        public event EventHandler<IPublicResource> ResourceAdded;

        private void RaiseResourceRemoved(IPublicResource newResource)
        {
            ResourceRemoved?.Invoke(this, newResource);
        }
        public event EventHandler<IPublicResource> ResourceRemoved;

        ///
        public event EventHandler<ICapabilities> CapabilitiesChanged;

        private void RaiseCapabilitiesChanged(object originalSender, ICapabilities capabilities)
        {
            // Only forward events for available resources
            if (Graph.GetWrapper(((IResource)originalSender).Id).State.IsAvailable)
                CapabilitiesChanged?.Invoke(originalSender, capabilities);
        }

        #endregion
    }
}
