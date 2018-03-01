using System;
using System.Collections.Generic;
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
using Newtonsoft.Json;

namespace Marvin.Resources.Management
{
    [Plugin(LifeCycle.Singleton, typeof(IResourceManager))]
    internal class ResourceManager : IResourceManager
    {
        #region Dependency Injection

        /// <summary>
        /// Type controller managing the type tree and proxy creation
        /// </summary>
        public IResourceTypeController TypeController { get; set; }

        /// <summary>
        /// Access to the database
        /// </summary>
        public IUnitOfWorkFactory UowFactory { get; set; }

        /// <summary>
        /// Error reporting in case a resource crashes
        /// </summary>
        public IModuleErrorReporting ErrorReporting { get; set; }

        /// <summary>
        /// Logger for tracing and errors
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Config of this module
        /// </summary>
        public ModuleConfig Config { get; set; }

        #endregion

        #region Fields

        /// <summary>
        /// Direct access to all resources of the tree
        /// </summary>
        private IDictionary<long, ResourceWrapper> _resources;

        /// <summary>
        /// Subset of public resources
        /// </summary>
        private List<IPublicResource> _publicResources;
        #endregion

        #region LifeCycle

        /// 
        public void Initialize()
        {
            using (var uow = UowFactory.Create(ContextMode.AllOff))
            {
                // Create all objects
                var allResources = ResourceCreationTemplate.FetchResourceTemplates(uow);
                if (allResources.Count > 0)
                    LoadResources(allResources);
                else
                    CreateRoot(uow);
            }

            // Boot resources
            Parallel.ForEach(_resources.Values, resourceWrapper =>
            {
                try
                {
                    resourceWrapper.Initialize();
                }
                catch (Exception e)
                {
                    resourceWrapper.ErrorOccured();
                    lock (_publicResources)
                        _publicResources.Remove(resourceWrapper.Target as IPublicResource);
                    ErrorReporting.ReportWarning(this, e);
                }
            });
        }

        /// <summary>
        /// Load and link all resources from the databse
        /// </summary>
        private void LoadResources(ICollection<ResourceCreationTemplate> allResources)
        {
            // Create dictionaries with initial capacity that should avoid the need of resizing
            _resources = new Dictionary<long, ResourceWrapper>(allResources.Count * 2);
            _publicResources = new List<IPublicResource>(allResources.Count);

            // Create resource objects on multiple threads
            var query = from template in allResources.AsParallel()
                        select template.Instantiate(TypeController, this);
            foreach (var resource in query)
            {
                AddResource(resource, false);
            }
            
            // Link them to each other
            Parallel.ForEach(allResources, LinkReferences);

            // Register events after all links were set
            foreach (var resourceWrapper in _resources.Values)
            {
                RegisterEvents(resourceWrapper.Target, resourceWrapper.Target as IPublicResource);
            }
        }

        /// <summary>
        /// Create root resource if the database is empty
        /// </summary>
        private void CreateRoot(IUnitOfWork uow)
        {
            // Create dictionaries with initial capacity that should avoid the need of resizing
            _resources = new Dictionary<long, ResourceWrapper>(64);
            _publicResources = new List<IPublicResource>(32);

            // Create a root resource
            var root = Create(Config.RootType);
            Save(uow, root);
            uow.Save();
            AddResource(root, true);
        }

        /// <summary>
        /// Add resource to all collections and register to the <see cref="Resource.Changed"/> event
        /// </summary>
        private void AddResource(Resource instance, bool registerEvents)
        {
            IPublicResource publicResource;

            lock (_resources)
            {
                // Add to collections
                _resources[instance.Id] = new ResourceWrapper(instance);
                publicResource = instance as IPublicResource;
                if (publicResource != null)
                    _publicResources.Add(publicResource);

                // Register to events
                if (registerEvents)
                    RegisterEvents(instance, publicResource);
            }

            RaiseResourceAdded(publicResource);
        }

        /// <summary>
        /// Register a resources events
        /// </summary>
        private void RegisterEvents(Resource instance, IPublicResource asPublic)
        {
            instance.Changed += OnResourceChanged;
            if (asPublic != null)
                asPublic.CapabilitiesChanged += RaiseCapabilitiesChanged;
        }

        /// <summary>
        /// Register a resources events
        /// </summary>
        private void UnregisterEvents(Resource instance, IPublicResource asPublic)
        {
            instance.Changed -= OnResourceChanged;
            if (asPublic != null)
                asPublic.CapabilitiesChanged -= RaiseCapabilitiesChanged;
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
        /// Build object graph from simplified <see cref="ResourceCreationTemplate"/> and flat resource list
        /// </summary>
        private void LinkReferences(ResourceCreationTemplate creationTemplate)
        {
            LinkReferences(creationTemplate.Instance, creationTemplate.Relations);
        }

        /// <summary>
        /// Link all references of a resource
        /// </summary>
        private void LinkReferences(Resource resource, ICollection<ResourceRelationTemplate> relations)
        {
            var resourceType = resource.GetType();
            foreach (var property in ReferenceProperties(resourceType))
            {
                var referenceOverride = property.GetCustomAttribute<ReferenceOverrideAttribute>();
                var isEnumerable = typeof(IEnumerable<IResource>).IsAssignableFrom(property.PropertyType);
                if (!isEnumerable && referenceOverride == null)
                {
                    // Link a single reference
                    var matches = MatchingRelations(relations, property);
                    if (matches.Count == 0)
                        continue;

                    // Try to find a possible match for the property
                    var propertyType = property.PropertyType;
                    var referenceMatch = (from match in matches
                                          let reference = _resources[match.ReferenceId].Target
                                          where propertyType.IsInstanceOfType(reference)
                                          select reference).ToArray();
                    if (referenceMatch.Length == 1)
                        property.SetValue(resource, referenceMatch[0]);
                    else
                        Logger.LogEntry(LogLevel.Warning, "Type mismatch: Can not assign any resource from [{0}] to {1} on {2}:{3} or too many matches!", string.Join(",", matches.Select(m => m.ReferenceId)), property.Name, resource.Id, resource.Name);
                }
                // Link a list of resources
                else if (isEnumerable && referenceOverride == null)
                {
                    // Read attribute and get the ReferenceCollection
                    var att = property.GetCustomAttribute<ResourceReferenceAttribute>();
                    var value = (IReferenceCollection)property.GetValue(resource);

                    var matches = MatchingRelations(relations, property);
                    var resources = matches.Select(m => _resources[m.ReferenceId].Target)
                        .OrderBy(r => r.LocalIdentifier).ThenBy(r => r.Name);
                    foreach (var referencedResource in resources)
                    {
                        value.UnderlyingCollection.Add(referencedResource);
                    }
                    if (att != null && att.AutoSave)
                        value.CollectionChanged += new SaveResourceTrigger(this, resource, property).OnCollectionChanged;
                }
                // Register on changes for ReferenceOverrides with AutoSave
                else if (isEnumerable && referenceOverride != null && referenceOverride.AutoSave)
                {
                    var target = resourceType.GetProperty(referenceOverride.Source);
                    var value = (IReferenceCollection)property.GetValue(resource);
                    // Reference override publish change for the source property instead
                    value.CollectionChanged += new SaveResourceTrigger(this, resource, target).OnCollectionChanged;
                }
            }
        }

        ///
        public void Start()
        {
            Parallel.ForEach(_resources.Values, resourceWrapper =>
            {
                try
                {
                    resourceWrapper.Start();
                }
                catch (Exception e)
                {
                    resourceWrapper.ErrorOccured();
                    lock (_publicResources)
                        _publicResources.Remove(resourceWrapper.Target as IPublicResource);
                    ErrorReporting.ReportWarning(this, e);
                }
            });
        }

        public void Stop()
        {
            Parallel.ForEach(_resources.Values, resourceWrapper =>
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
        }

        ///
        public void Dispose()
        {
            foreach (var resourceWrapper in _resources.Values)
            {
                UnregisterEvents(resourceWrapper.Target, resourceWrapper.Target as IPublicResource);
            }
        }

        #endregion

        public Resource Get(long id) => _resources[id].Target;

        public Resource Create(string type)
        {
            // Create simplified template and instantiate
            var template = new ResourceCreationTemplate();
            template.Name = template.Type = type; // Initially set name to type
            var instance = template.Instantiate(TypeController, this);

            // Provide ReferenceCollections for the new instance
            LinkReferences(instance, new List<ResourceRelationTemplate>());

            return instance;
        }

        public void Save(Resource resource)
        {
            using (var uow = UowFactory.Create())
            {
                Save(uow, resource);
                uow.Save();
            }
        }

        /// <summary>
        /// A collection with "AutoSave = true" was modified. Write current state to the database
        /// </summary>
        private void AutoSaveCollection(Resource instance, PropertyInfo collectionProperty)
        {
            using (var uow = UowFactory.Create())
            {
                var entity = uow.GetEntity<ResourceEntity>(instance);
                var relations = ResourceRelationTemplate.FromEntity(uow, entity);
                var matches = MatchingRelations(relations, collectionProperty);
                UpdateCollectionReference(uow, entity, instance, collectionProperty, matches);
                uow.Save();
            }
        }

        /// <summary>
        /// Save a resource to the database
        /// </summary>
        private ResourceEntity Save(IUnitOfWork uow, Resource resource)
        {
            // Create entity and populate from object
            var entity = uow.GetEntity<ResourceEntity>(resource);
            if (entity.Id == 0)
            {
                entity.Type = resource.GetType().Name;
                LinkReferences(resource, new List<ResourceRelationTemplate>()); // Register on references for new instance
                EntityIdListener.Listen(entity, new SaveResourceTrigger(this, resource));
            }

            entity.Name = resource.Name;
            entity.LocalIdentifier = resource.LocalIdentifier;
            entity.GlobalIdentifier = resource.GlobalIdentifier;
            entity.ExtensionData = JsonConvert.SerializeObject(resource, JsonSettings.Minimal);

            // Save references
            var relations = ResourceRelationTemplate.FromEntity(uow, entity);
            foreach (var referenceProperty in ReferenceProperties(resource.GetType(), false))
            {
                var matches = MatchingRelations(relations, referenceProperty);
                if (typeof(IEnumerable<IResource>).IsAssignableFrom(referenceProperty.PropertyType))
                {
                    // Save a collection reference
                    UpdateCollectionReference(uow, entity, resource, referenceProperty, matches);
                }
                else
                {
                    // Save a single reference
                    UpdateSingleReference(uow, entity, resource, referenceProperty, matches);
                }
            }

            return entity;
        }

        /// <summary>
        /// Make sure our resource-relation graph in the database is synced to the resource object graph. This method
        /// updates single references like in the example below
        /// </summary>
        /// <example>
        /// [ResourceReference(ResourceRelationType.TransportRoute, ResourceReferenceRole.Source)]
        /// public Resource FriendResource { get; set; }
        /// </example>
        private void UpdateSingleReference(IUnitOfWork uow, ResourceEntity entity, Resource resource, PropertyInfo referenceProperty, IReadOnlyList<ResourceRelationTemplate> matches)
        {
            var relationRepo = uow.GetRepository<IResourceRelationRepository>();

            var value = referenceProperty.GetValue(resource);
            var referencedResource = value as Resource;
            // Validate if object assigned to the property is a resource
            if (value != null && referencedResource == null)
                throw new ArgumentException($"Value of property {referenceProperty.Name} on resource {resource.Id}:{resource.GetType().Name} must be a Resource");

            var att = referenceProperty.GetCustomAttribute<ResourceReferenceAttribute>();

            // Try to find a match that previously referenced another compatible resource
            var propertyType = referenceProperty.PropertyType;
            var relEntity = (from match in matches
                             where propertyType.IsInstanceOfType(_resources[match.ReferenceId].Target)
                             select match.Entity).SingleOrDefault();
            if (relEntity == null && referencedResource != null)
            {
                // Create a new relation
                relEntity = CreateRelationForProperty(relationRepo, att);
            }
            else if (relEntity != null && referencedResource == null)
            {
                // Delete a relation, that no longer exists
                relationRepo.Remove(relEntity);
                return;
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse <<- To identify the remaining case
            else if (relEntity == null && referencedResource == null)
            {
                // Relation did not exist before and still does not
                return;
            }

            // Set source and target of the relation depending on the reference roles
            var referencedEntity = referencedResource.Id > 0 ? uow.GetEntity<ResourceEntity>(referencedResource) : Save(uow, referencedResource);
            UpdateRelationEntity(entity, referencedEntity, relEntity, att);
        }

        /// <summary>
        /// Make sure our resource-relation graph in the database is synced to the resource object graph. This method
        /// updates a collection of references
        /// </summary>
        /// <example>
        /// [ResourceReference(ResourceRelationType.TransportRoute, ResourceReferenceRole.Source)]
        /// public IReferences&lt;Resource&gt; FriendResources { get; set; }
        /// </example>
        private void UpdateCollectionReference(IUnitOfWork uow, ResourceEntity entity, Resource resource, PropertyInfo referenceProperty, IReadOnlyList<ResourceRelationTemplate> relationTemplates)
        {
            var relationRepo = uow.GetRepository<IResourceRelationRepository>();
            var referenceAtt = referenceProperty.GetCustomAttribute<ResourceReferenceAttribute>();

            // Get the value stored in the reference property
            var propertyValue = referenceProperty.GetValue(resource);
            var referencedResources = ((IEnumerable<IResource>)propertyValue).Cast<Resource>().ToList();

            // First delete references that no longer exist
            var deleted = relationTemplates.Where(m => referencedResources.All(r => r.Id != m.ReferenceId)).Select(m => m.Entity);
            relationRepo.RemoveRange(deleted);

            // Now create new relations
            var created = referencedResources.Where(r => relationTemplates.All(m => m.ReferenceId != r.Id));
            foreach (var createdReference in created)
            {
                var referencedEntity = createdReference.Id > 0 ? uow.GetEntity<ResourceEntity>(createdReference) : Save(uow, createdReference);
                var relEntity = CreateRelationForProperty(relationRepo, referenceAtt);
                UpdateRelationEntity(entity, referencedEntity, relEntity, referenceAtt);
            }
        }

        /// <summary>
        /// Create a <see cref="ResourceRelation"/> entity for a property match
        /// </summary>
        private static ResourceRelation CreateRelationForProperty(IResourceRelationRepository relationRepo, ResourceReferenceAttribute att)
        {
            var relationType = att.RelationType;
            var relEntity = relationRepo.Create((int)relationType);
            if (!string.IsNullOrEmpty(att.Name))
                relEntity.RelationName = att.Name;
            return relEntity;
        }

        /// <summary>
        /// Set <see cref="ResourceRelation.SourceId"/> and <see cref="ResourceRelation.TargetId"/> depending on the <see cref="ResourceReferenceRole"/>
        /// of the reference property
        /// </summary>
        private static void UpdateRelationEntity(ResourceEntity resource, ResourceEntity referencedResource, ResourceRelation relEntity, ResourceReferenceAttribute att)
        {
            if (att.Role == ResourceReferenceRole.Source)
            {
                relEntity.Source = referencedResource;
                relEntity.Target = resource;
            }
            else
            {
                relEntity.Source = resource;
                relEntity.Target = referencedResource;
            }
        }

        private static IEnumerable<PropertyInfo> ReferenceProperties(Type resourceType, bool includeOverrides = true)
        {
            return (from property in resourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    where property.CanWrite && Attribute.IsDefined(property, typeof(ResourceReferenceAttribute))
                       || includeOverrides && Attribute.IsDefined(property, typeof(ReferenceOverrideAttribute))
                    select property);
        }

        /// <summary>
        /// Find the relation that matches the property
        /// </summary>
        private static IReadOnlyList<ResourceRelationTemplate> MatchingRelations(IEnumerable<ResourceRelationTemplate> relations, PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<ResourceReferenceAttribute>();
            var matches = (from relation in relations
                           where attribute.Role == relation.Role
                           where attribute.RelationType == relation.RelationType // Typed relation without name or matching name
                                && (string.IsNullOrEmpty(attribute.Name) || attribute.Name == relation.Name)
                           select relation);
            return matches.ToArray();
        }

        ///
        public bool Start(Resource resource)
        {
            try
            {
                resource.Start();
                return true;
            }
            catch (Exception e)
            {
                ErrorReporting.ReportWarning(this, e);
                return false;
            }
        }

        ///
        public bool Stop(Resource resource)
        {
            try
            {
                resource.Stop();
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
            return (TResource)Instantiate(typeof(TResource).Name);
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
            instance.Stop();

            // Load entity and relations to disconnect resource and remove from database
            using (var uow = UowFactory.Create())
            {
                var resourceRepository = uow.GetRepository<IResourceEntityRepository>();
                var relationRepository = uow.GetRepository<IResourceRelationRepository>();

                // Fetch entity and relations
                // Update properties on the references and get rid of relation entities
                var entity = resourceRepository.GetByKey(instance.Id);
                foreach (var relationEntity in entity.Sources.Concat(entity.Targets).ToArray())
                {
                    var referenceId = relationEntity.SourceId == instance.Id
                        ? relationEntity.TargetId
                        : relationEntity.SourceId;
                    var reference = _resources[referenceId].Target;

                    var property = GetProperty(reference, instance);
                    if (property != null)
                        UpdateProperty(reference, instance, property);

                    if (permanent)
                        relationRepository.Remove(relationEntity);
                }

                resourceRepository.Remove(entity, permanent);

                uow.Save();
            }

            // Unregister from all events to avoid memory leaks
            UnregisterEvents(instance, instance as IPublicResource);

            // Destroy the object
            TypeController.Destroy(instance);

            // Remove from internal collections
            return _publicResources.Remove(resource as IPublicResource) | _resources.Remove(instance.Id);
        }

        private static PropertyInfo GetProperty(IResource referencedResource, IResource instance)
        {
            var type = referencedResource.GetType();
            return (from property in ReferenceProperties(type)
                        // Instead of comparing the resource type we simply look for the object reference
                    let value = property.GetValue(referencedResource)
                    where value == instance || ((value as IEnumerable<IResource>)?.Contains(instance) ?? false)
                    select property).FirstOrDefault();
        }

        private static void UpdateProperty(Resource reference, Resource instance, PropertyInfo property)
        {
            if (typeof(IEnumerable<IResource>).IsAssignableFrom(property.PropertyType))
            {
                var referenceCollection = (IReferenceCollection)property.GetValue(reference);
                referenceCollection.UnderlyingCollection.Remove(instance);
            }
            else
            {
                property.SetValue(reference, null);
            }
        }

        #endregion

        #region IResourceManagement

        public TResource GetResource<TResource>() where TResource : class, IPublicResource
        {
            return GetResource<TResource>(r => true);
        }

        public TResource GetResource<TResource>(long id) where TResource : class, IPublicResource
        {
            return GetResource<TResource>(r => r.Id == id);
        }

        public TResource GetResource<TResource>(string name) where TResource : class, IPublicResource
        {
            return GetResource<TResource>(r => r.Name == name);
        }

        public TResource GetResource<TResource>(ICapabilities requiredCapabilities) where TResource : class, IPublicResource
        {
            return GetResource<TResource>(r => requiredCapabilities.ProvidedBy(r.Capabilities));
        }

        public TResource GetResource<TResource>(Func<TResource, bool> predicate)
            where TResource : class, IPublicResource
        {
            // Public resources without capabilities are considered non-public
            var match = _publicResources.OfType<TResource>().SingleOrDefault(r => r.Capabilities != NullCapabilities.Instance && predicate(r));
            if (match == null)
                throw new ResourceNotFoundException();

            return (TResource)TypeController.GetProxy(match as Resource);
        }

        public IEnumerable<TResource> GetResources<TResource>() where TResource : class, IPublicResource
        {
            return GetResources<TResource>(r => true);
        }

        public IEnumerable<TResource> GetResources<TResource>(ICapabilities requiredCapabilities) where TResource : class, IPublicResource
        {
            return GetResources<TResource>(r => requiredCapabilities.ProvidedBy(r.Capabilities));
        }

        public IEnumerable<TResource> GetResources<TResource>(Func<TResource, bool> predicate) where TResource : class, IPublicResource
        {
            return _publicResources.OfType<TResource>().Where(r => r.Capabilities != NullCapabilities.Instance)
                .Where(predicate).Select(r => TypeController.GetProxy(r as Resource)).Cast<TResource>();
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
            var senderResource = (Resource)originalSender;
            var senderProxy = TypeController.GetProxy(senderResource);
            CapabilitiesChanged?.Invoke(senderProxy, capabilities);
        }
        #endregion

        /// <summary>
        /// Save resource trigger that forwards an event back to the
        /// <see cref="ResourceManager"/> to save the instance
        /// </summary>
        private class SaveResourceTrigger : EntityIdListener
        {
            private readonly ResourceManager _parent;
            private readonly Resource _instance;
            private readonly PropertyInfo _referenceProperty;

            public SaveResourceTrigger(ResourceManager parent, Resource instance, PropertyInfo referenceProperty = null)
            {
                _parent = parent;
                _instance = instance;
                _referenceProperty = referenceProperty;
            }

            protected override void AssignId(long id)
            {
                _parent.AddResource(_instance, true);
            }

            internal void OnCollectionChanged(object sender, EventArgs e)
            {
                _parent.AutoSaveCollection(_instance, _referenceProperty);
            }
        }
    }
}