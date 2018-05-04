using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marvin.AbstractionLayer.Resources;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Model;
using Marvin.Resources.Model;
using Marvin.Tools;

namespace Marvin.Resources.Management
{
    [Component(LifeCycle.Singleton, typeof(IResourceLinker))]
    internal class ResourceLinker : IResourceLinker
    {
        [UseChild("ResourceLinker")]
        public IModuleLogger Logger { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<Resource> SaveRoots(IUnitOfWork uow, IReadOnlyList<Resource> instances)
        {
            var context = new ReferenceSaverContext(uow);
            foreach (var instance in instances)
            {
                SaveReferences(context, instance);
            }
            return context.EntityCache.Keys.ToArray();
        }

        /// <inheritdoc />
        public void SetReferenceCollections(Resource instance)
        {
            var resourceType = instance.GetType();
            // Iterate all references and provide reference collections
            var overrides = new Dictionary<PropertyInfo, ReferenceOverrideAttribute>();
            foreach (var property in CollectionReferenceProperties(resourceType))
            {
                var attribute = property.GetCustomAttribute<ReferenceOverrideAttribute>();
                if (attribute == null)
                    // Create collection and set on property
                    CreateCollection(instance, property);
                else
                    // Save overrides for later
                    overrides[property] = attribute;
            }

            // Now set the reference overrides
            foreach (var pair in overrides)
            {
                // Fetch already created reference collection
                var targetName = pair.Value.Source;
                var target = resourceType.GetProperty(targetName);
                var sourceCollection = (IReferenceCollection)target.GetValue(instance);

                // Create new reference collection that shares the UnderlyingCollection
                var property = pair.Key;
                CreateCollection(instance, property, sourceCollection.UnderlyingCollection, target);
            }
        }

        /// <summary>
        /// Create a <see cref="ReferenceCollection{TResource}"/> instance
        /// </summary>
        /// <param name="instance">The resource instance to create the collection for</param>
        /// <param name="property">The collection property that should be filled by this collection</param>
        /// <param name="underlyingCollection">The base collection wrapped in the reference collection. This can be null for non-override properties</param>
        /// <param name="targetProperty">Target property of the collection. For non-overrides this equals <paramref name="property"/>.</param>
        private static void CreateCollection(Resource instance, PropertyInfo property, ICollection<IResource> underlyingCollection = null, PropertyInfo targetProperty = null)
        {
            // Set target property to property if it is not given
            if (targetProperty == null)
                targetProperty = property;

            // Create underlying collection if it is not given
            if (underlyingCollection == null)
                underlyingCollection = new SynchronizedCollection<IResource>();

            var propertyType = property.PropertyType;
            var referenceType = propertyType.GetGenericArguments()[0]; // Type of resource from ICollection<ResourceType>
            var collectionType = typeof(ReferenceCollection<>).MakeGenericType(referenceType); // Make generic ReferenceCollection

            // Create collection and set on instance property
            var value = Activator.CreateInstance(collectionType, instance, targetProperty, underlyingCollection);
            property.SetValue(instance, value);
        }

        /// <inheritdoc />
        public ICollection<IReferenceCollection> GetAutoSaveCollections(Resource instance)
        {
            return (from collectionProperty in CollectionReferenceProperties(instance.GetType())
                    let refAtt = collectionProperty.GetCustomAttribute<ResourceReferenceAttribute>()
                    let overrideAtt = collectionProperty.GetCustomAttribute<ReferenceOverrideAttribute>()
                    where (refAtt?.AutoSave ?? false) || (overrideAtt?.AutoSave ?? false)
                    select (IReferenceCollection)collectionProperty.GetValue(instance)).ToList();
        }

        /// <inheritdoc />
        public void LinkReferences(Resource resource, ICollection<ResourceRelationAccessor> relations, IDictionary<long, ResourceWrapper> allResources)
        {
            var resourceType = resource.GetType();
            foreach (var property in ReferenceProperties(resourceType, false))
            {
                // Link a list of resources
                if (typeof(IEnumerable<IResource>).IsAssignableFrom(property.PropertyType))
                {
                    // Read attribute and get the ReferenceCollection
                    var value = (IReferenceCollection)property.GetValue(resource);
                    var elemType = property.PropertyType.GetGenericArguments()[0];

                    var matches = MatchingRelations(relations, property);
                    var resources = matches.Select(m => allResources[m.ReferenceId].Target)
                        .Where(elemType.IsInstanceOfType)
                        .OrderBy(r => r.LocalIdentifier).ThenBy(r => r.Name);

                    foreach (var referencedResource in resources)
                    {
                        value.UnderlyingCollection.Add(referencedResource);
                    }
                }
                // Link a single reference
                else
                {
                    var matches = MatchingRelations(relations, property);
                    if (matches.Count == 0)
                        continue;

                    // Try to find a possible match for the property
                    var propertyType = property.PropertyType;
                    var referenceMatch = (from match in matches
                                          let reference = allResources[match.ReferenceId].Target
                                          where propertyType.IsInstanceOfType(reference)
                                          select reference).ToArray();
                    if (referenceMatch.Length == 1)
                        property.SetValue(resource, referenceMatch[0]);
                    else
                        Logger.LogEntry(LogLevel.Warning, "Type mismatch: Can not assign any resource from [{0}] to {1} on {2}:{3} or too many matches!", string.Join(",", matches.Select(m => m.ReferenceId)), property.Name, resource.Id, resource.Name);

                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<Resource> SaveReferences(IUnitOfWork uow, Resource instance, ResourceEntity entity)
        {
            var context = new ReferenceSaverContext(uow, instance, entity);
            SaveReferences(context, instance);
            return context.EntityCache.Keys.Where(i => i.Id == 0);
        }

        private static void SaveReferences(ReferenceSaverContext context, Resource instance)
        {
            var entity = GetOrCreateEntity(context, instance);

            var relations = ResourceRelationAccessor.FromEntity(context.UnitOfWork, entity)
                .Union(ResourceRelationAccessor.FromQueryable(context.CreatedRelations.AsQueryable(), entity))
                .ToList();

            var createdResources = new List<Resource>();
            foreach (var referenceProperty in ReferenceProperties(instance.GetType(), false))
            {
                var matches = MatchingRelations(relations, referenceProperty);
                if (typeof(IEnumerable<IResource>).IsAssignableFrom(referenceProperty.PropertyType))
                {
                    // Save a collection reference
                    var created = UpdateCollectionReference(context, entity, instance, referenceProperty, matches);
                    createdResources.AddRange(created);
                }
                else
                {
                    // Save a single reference
                    var createdResource = UpdateSingleReference(context, entity, instance, referenceProperty, matches);
                    if (createdResource != null)
                        createdResources.Add(createdResource);
                }
            }

            // Recursively save references for new resources
            foreach (var resource in createdResources)
                SaveReferences(context, resource);
        }

        /// <inheritdoc />
        public IEnumerable<Resource> SaveSingleCollection(IUnitOfWork uow, Resource instance, PropertyInfo property)
        {
            var entity = uow.GetEntity<ResourceEntity>(instance);
            var relations = ResourceRelationAccessor.FromEntity(uow, entity);
            var matches = MatchingRelations(relations, property);

            var context = new ReferenceSaverContext(uow, instance, entity);
            UpdateCollectionReference(context, entity, instance, property, matches);
            return context.EntityCache.Keys.Where(i => i.Id == 0);
        }

        /// <summary>
        /// Make sure our resource-relation graph in the database is synced to the resource object graph. This method
        /// updates single references like in the example below
        /// </summary>
        /// <example>
        /// [ResourceReference(ResourceRelationType.TransportRoute, ResourceReferenceRole.Source)]
        /// public Resource FriendResource { get; set; }
        /// </example>
        private static Resource UpdateSingleReference(ReferenceSaverContext context, ResourceEntity entity, Resource resource, PropertyInfo referenceProperty, IReadOnlyList<ResourceRelationAccessor> matches)
        {
            var relationRepo = context.UnitOfWork.GetRepository<IResourceRelationRepository>();

            var value = referenceProperty.GetValue(resource);
            var referencedResource = value as Resource;
            // Validate if object assigned to the property is a resource
            if (value != null && referencedResource == null)
                throw new ArgumentException($"Value of property {referenceProperty.Name} on resource {resource.Id}:{resource.GetType().Name} must be a Resource");

            // Check if there is a relation that represents this reference
            if (referencedResource != null && matches.Any(m => m.ReferenceId == referencedResource.Id))
                return null;

            var referenceAtt = referenceProperty.GetCustomAttribute<ResourceReferenceAttribute>();
            // Get all references of this resource with the same relation type
            var currentReferences = (from property in ReferenceProperties(resource.GetType(), false)
                                     let att = property.GetCustomAttribute<ResourceReferenceAttribute>()
                                     where att.RelationType == referenceAtt.RelationType
                                        && att.Name == referenceAtt.Name
                                        && att.Role == referenceAtt.Role
                                     select property.GetValue(resource)).Distinct().OfType<Resource>().ToList();
            // Try to find a match that is not used in any reference
            var relEntity = (from match in matches
                             where currentReferences.All(cr => cr.Id != match.ReferenceId)
                             select match.Entity).FirstOrDefault();
            if (relEntity == null && referencedResource != null)
            {
                // Create a new relation
                relEntity = CreateRelationForProperty(context, relationRepo, referenceAtt);
            }
            else if (relEntity != null && referencedResource == null)
            {
                // Delete a relation, that no longer exists
                relationRepo.Remove(relEntity);
                return null;
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse <<- To identify the remaining case
            else if (relEntity == null && referencedResource == null)
            {
                // Relation did not exist before and still does not
                return null;
            }

            // Set source and target of the relation depending on the reference roles
            var referencedEntity = GetOrCreateEntity(context, referencedResource);
            UpdateRelationEntity(entity, referencedEntity, relEntity, referenceAtt);

            // Return referenced resource if it is new
            return referencedResource.Id == 0 ? referencedResource : null;
        }

        /// <summary>
        /// Make sure our resource-relation graph in the database is synced to the resource object graph. This method
        /// updates a collection of references
        /// </summary>
        /// <example>
        /// [ResourceReference(ResourceRelationType.TransportRoute, ResourceReferenceRole.Source)]
        /// public IReferences&lt;Resource&gt; FriendResources { get; set; }
        /// </example>
        private static IEnumerable<Resource> UpdateCollectionReference(ReferenceSaverContext context, ResourceEntity entity, Resource resource, PropertyInfo referenceProperty, IReadOnlyList<ResourceRelationAccessor> relationTemplates)
        {
            var relationRepo = context.UnitOfWork.GetRepository<IResourceRelationRepository>();
            var referenceAtt = referenceProperty.GetCustomAttribute<ResourceReferenceAttribute>();

            // Get the value stored in the reference property
            var propertyValue = referenceProperty.GetValue(resource);
            var referencedResources = ((IEnumerable<IResource>)propertyValue).Cast<Resource>().ToList();

            // First delete references that no longer exist
            var deleted = relationTemplates.Where(m => referencedResources.All(r => r.Id != m.ReferenceId)).Select(m => m.Entity);
            relationRepo.RemoveRange(deleted);

            // Now create new relations
            var created = referencedResources.Where(r => relationTemplates.All(m => m.ReferenceId != r.Id)).ToList();
            foreach (var createdReference in created)
            {
                var relEntity = CreateRelationForProperty(context, relationRepo, referenceAtt);
                var referencedEntity = GetOrCreateEntity(context, createdReference);
                UpdateRelationEntity(entity, referencedEntity, relEntity, referenceAtt);
            }

            return created.Where(cr => cr.Id == 0);
        }

        /// <summary>
        /// Get or create an entity for a resource instance
        /// </summary>
        private static ResourceEntity GetOrCreateEntity(ReferenceSaverContext context, Resource instance)
        {
            // First check if the context contains an entity for the instance
            if (context.EntityCache.ContainsKey(instance))
                return context.EntityCache[instance];

            // Get or create an entity for the instance
            return context.EntityCache[instance] = instance.Id > 0
                ? context.UnitOfWork.GetEntity<ResourceEntity>(instance)
                : ResourceEntityAccessor.SaveToEntity(context.UnitOfWork, instance);
        }

        /// <summary>
        /// Create a <see cref="ResourceRelation"/> entity for a property match
        /// </summary>
        private static ResourceRelation CreateRelationForProperty(ReferenceSaverContext context, IResourceRelationRepository relationRepo, ResourceReferenceAttribute att)
        {
            var relationType = att.RelationType;
            var relEntity = relationRepo.Create((int)relationType);
            if (!string.IsNullOrEmpty(att.Name))
                relEntity.RelationName = att.Name;

            context.CreatedRelations.Add(relEntity);

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

        /// <inheritdoc />
        public void RemoveLinking(IResource deletedInstance, IResource reference)
        {
            // Try to find a property on the reference back-linking to the deleted instance
            var type = reference.GetType();
            var backReference = (from property in ReferenceProperties(type, false)
                                     // Instead of comparing the resource type we simply look for the object reference
                                 let value = property.GetValue(reference)
                                 where value == deletedInstance || ((value as IEnumerable<IResource>)?.Contains(deletedInstance) ?? false)
                                 select property).FirstOrDefault();

            // If the referenced resource does not define a back reference we don't have to do anything
            if (backReference == null)
                return;

            // Remove the reference from the property
            if (typeof(IEnumerable<IResource>).IsAssignableFrom(backReference.PropertyType))
            {
                var referenceCollection = (IReferenceCollection)backReference.GetValue(reference);
                referenceCollection.UnderlyingCollection.Remove(deletedInstance);
            }
            else
            {
                backReference.SetValue(reference, null);
            }
        }

        /// <summary>
        /// Find the relation that matches the property
        /// </summary>
        private static IReadOnlyList<ResourceRelationAccessor> MatchingRelations(IEnumerable<ResourceRelationAccessor> relations, PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<ResourceReferenceAttribute>();
            var matches = (from relation in relations
                           where attribute.Role == relation.Role
                           where attribute.RelationType == relation.RelationType // Typed relation without name or matching name
                                 && attribute.Name == relation.Name
                           select relation);
            return matches.ToArray();
        }

        /// <summary>
        /// Find all reference properties on a resource type
        /// </summary>
        private static IEnumerable<PropertyInfo> CollectionReferenceProperties(Type resourceType)
        {
            return from referenceProperty in ReferenceProperties(resourceType, true)
                   let propertyType = referenceProperty.PropertyType
                   where propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IReferences<>)
                   select referenceProperty;
        }

        /// <summary>
        /// All properties of a resource type that represent references or reference overrides
        /// </summary>
        private static IEnumerable<PropertyInfo> ReferenceProperties(Type resourceType, bool includeOverrides)
        {
            return from property in resourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where property.CanWrite &&
                         (Attribute.IsDefined(property, typeof(ResourceReferenceAttribute))
                          || includeOverrides && Attribute.IsDefined(property, typeof(ReferenceOverrideAttribute)))
                   select property;
        }

        /// <summary>
        /// Context for the recursive save references structure
        /// </summary>
        private struct ReferenceSaverContext
        {
            public ReferenceSaverContext(IUnitOfWork unitOfWork, Resource initialInstance, ResourceEntity entity) : this(unitOfWork)
            {
                EntityCache[initialInstance] = entity;
            }

            public ReferenceSaverContext(IUnitOfWork uow)
            {
                UnitOfWork = uow;
                EntityCache = new Dictionary<Resource, ResourceEntity>();
                CreatedRelations = new List<ResourceRelation>();
            }

            /// <summary>
            /// Open database context of this operation
            /// </summary>
            public IUnitOfWork UnitOfWork { get; }

            /// <summary>
            /// Cache of instances to entities
            /// </summary>
            public IDictionary<Resource, ResourceEntity> EntityCache { get; }

            /// <summary>
            /// Accesor wrappers for relations that were created while saving references
            /// </summary>
            public IList<ResourceRelation> CreatedRelations { get; }
        }
    }
}