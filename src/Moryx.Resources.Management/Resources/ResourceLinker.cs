// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Model;
using Moryx.Model.Repositories;
using Moryx.Resources.Model;
using static Moryx.Resources.Management.ResourceReferenceTools;

namespace Moryx.Resources.Management
{
    [Component(LifeCycle.Singleton, typeof(IResourceLinker))]
    internal class ResourceLinker : IResourceLinker
    {
        [UseChild("ResourceLinker")]
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Resource graph with all resources
        /// </summary>
        public IResourceGraph Graph { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<Resource> SaveRoots(IUnitOfWork uow, IReadOnlyList<Resource> instances)
        {
            var context = new ReferenceSaverContext(uow, Graph);
            foreach (var instance in instances)
            {
                SaveReferences(context, instance);
            }
            return context.EntityCache.Keys.ToArray();
        }

        /// <inheritdoc />
        public void LinkReferences(Resource resource, ICollection<ResourceRelationAccessor> relations)
        {
            var resourceType = resource.GetType();
            var referenceProperties = ReferenceProperties(resourceType, false).ToList();
            // Create delegate once to optimize memory usage
            var resolverDelegate = new Func<ResourceRelationAccessor, Resource>(ResolveRefernceWithGraph);
            foreach (var property in referenceProperties)
            {
                var matches = MatchingRelations(relations, property).ToList();
                var resources = InstanceProjection(matches, property, resolverDelegate)
                    .OrderBy(r => r.Name).ToList();

                // Link a list of resources
                if (typeof(IEnumerable<IResource>).IsAssignableFrom(property.PropertyType))
                {
                    // Get the reference collection
                    var value = (IReferenceCollection)property.GetValue(resource);
                    foreach (var referencedResource in resources)
                    {
                        value.UnderlyingCollection.Add(referencedResource);
                    }
                }
                // Link a single reference
                else
                {
                    if (resources.Count == 1)
                        property.SetValue(resource, resources[0]);
                    else if (resources.Count > 1)
                        Logger.Log(LogLevel.Warning, "Inconclusive relation: Can not assign property {0} on {1}:{2} from [{3}]. Too many matches!", property.Name, resource.Id, resource.Name, string.Join(",", matches.Select(m => m.ReferenceId)));
                    else if (matches.Any(m => referenceProperties.All(p => !PropertyMatchesRelation(p, m, Graph.Get(m.ReferenceId)))))
                        Logger.Log(LogLevel.Warning, "Incompatible relation: Resources from [{0}] with relation type {1} can not be assigned to a property on {2}:{3}.", string.Join(",", matches.Select(m => m.ReferenceId)), matches[0].RelationType, resource.Id, resource.Name);
                }
            }
        }

        private static bool PropertyMatchesRelation(PropertyInfo property, ResourceRelationAccessor relation, Resource instance)
        {
            var att = property.GetCustomAttribute<ResourceReferenceAttribute>();
            return att.Role == relation.Role && att.RelationType == relation.RelationType && att.Name == relation.Name
                && property.PropertyType.IsInstanceOfType(instance);
        }

        /// <inheritdoc />
        public IReadOnlyList<Resource> SaveReferences(IUnitOfWork uow, Resource instance, ResourceEntity entity, Dictionary<Resource, ResourceEntity> dict = null)
        {
            var context = new ReferenceSaverContext(uow, Graph, instance, entity);
            SaveReferences(context, instance, dict);
            return context.EntityCache.Keys.Where(i => i.Id == 0).ToList();
        }

        private void SaveReferences(ReferenceSaverContext context, Resource instance, Dictionary<Resource, ResourceEntity> dict = null)
        {
            var entity = GetOrCreateEntity(context, instance);
            if (dict != null)
                dict.Add(instance, entity);

            var referenceAccessors = ResourceRelationAccessor.FromEntity(context.UnitOfWork, entity)
                .Union(ResourceRelationAccessor.FromQueryable(context.CreatedRelations.AsQueryable(), entity))
                .ToList();

            var createdResources = new List<Resource>();
            foreach (var referenceProperty in ReferenceProperties(instance.GetType(), false))
            {
                var matches = MatchingRelations(referenceAccessors, referenceProperty);
                var typeMatches = TypeFilter(matches, referenceProperty, context.ResolveReferencedResource).ToList();
                if (typeof(IEnumerable<IResource>).IsAssignableFrom(referenceProperty.PropertyType))
                {
                    // Save a collection reference
                    var created = UpdateCollectionReference(context, entity, instance, referenceProperty, typeMatches);
                    createdResources.AddRange(created);
                }
                else
                {
                    // Save a single reference
                    var createdResource = UpdateSingleReference(context, entity, instance, referenceProperty, typeMatches);
                    if (createdResource != null)
                        createdResources.Add(createdResource);
                }
            }

            // Recursively save references for new resources
            foreach (var resource in createdResources)
                SaveReferences(context, resource, dict);
        }

        /// <inheritdoc />
        public IReadOnlyList<Resource> SaveSingleCollection(IUnitOfWork uow, Resource instance, PropertyInfo property)
        {
            var entity = uow.GetEntity<ResourceEntity>(instance);
            var relations = ResourceRelationAccessor.FromEntity(uow, entity);

            var context = new ReferenceSaverContext(uow, Graph, instance, entity);
            var matches = MatchingRelations(relations, property);
            var typeMatches = TypeFilter(matches, property, context.ResolveReferencedResource).ToList();
            var created = UpdateCollectionReference(context, entity, instance, property, typeMatches);

            foreach (var resource in created)
                SaveReferences(context, resource);

            return context.EntityCache.Keys.Where(i => i.Id == 0).ToList();
        }

        /// <summary>
        /// Make sure our resource-relation graph in the database is synced to the resource object graph. This method
        /// updates single references like in the example below
        /// </summary>
        /// <example>
        /// [ResourceReference(ResourceRelationType.TransportRoute, ResourceReferenceRole.Source)]
        /// public Resource FriendResource { get; set; }
        /// </example>
        private Resource UpdateSingleReference(ReferenceSaverContext context, ResourceEntity entity, Resource resource, PropertyInfo referenceProperty, IReadOnlyList<ResourceRelationAccessor> matches)
        {
            var relationRepo = context.UnitOfWork.GetRepository<IResourceRelationRepository>();

            var value = referenceProperty.GetValue(resource);
            var referencedResource = value as Resource;
            // Validate if object assigned to the property is a resource
            if (value != null && referencedResource == null)
                throw new ArgumentException($"Value of property {referenceProperty.Name} on resource {resource.Id}:{resource.GetType().Name} must be a Resource");

            var referenceAtt = referenceProperty.GetCustomAttribute<ResourceReferenceAttribute>();

            // Validate if required property is set
            if (referencedResource == null && referenceAtt.IsRequired)
                throw new ValidationException($"Property {referenceProperty.Name} is flagged 'Required' and was null!");

            // Check if there is a relation that represents this reference
            if (referencedResource != null && matches.Any(m => referencedResource == context.ResolveReferencedResource(m)))
                return null;

            // Get all references of this resource with the same relation information
            var currentReferences = CurrentReferences(resource, referenceAtt);

            // Try to find a match that is not used in any reference
            var relMatch = (from match in matches
                            where currentReferences.All(cr => cr != context.ResolveReferencedResource(match))
                            select match).FirstOrDefault();
            var relEntity = relMatch?.Entity;
            if (relEntity == null && referencedResource != null)
            {
                // Create a new relation
                relEntity = CreateRelationForProperty(context, relationRepo, referenceAtt);
                SetOnTarget(referencedResource, resource, referenceAtt, relEntity);
            }
            else if (relEntity != null && referencedResource == null)
            {
                // Delete a relation, that no longer exists
                ClearOnTarget(context.ResolveReferencedResource(relMatch), resource, referenceAtt);
                relationRepo.Remove(relEntity);
                return null;
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse <<- To identify the remaining case
            else if (relEntity == null && referencedResource == null)
            {
                // Relation did not exist before and still does not
                return null;
            }
            // Relation was updated, make sure the backlinks match
            else
            {
                ClearOnTarget(context.ResolveReferencedResource(relMatch), resource, referenceAtt);
                SetOnTarget(referencedResource, resource, referenceAtt, relEntity);
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
        private IEnumerable<Resource> UpdateCollectionReference(ReferenceSaverContext context, ResourceEntity entity, Resource resource, PropertyInfo referenceProperty, IReadOnlyList<ResourceRelationAccessor> relationTemplates)
        {
            var relationRepo = context.UnitOfWork.GetRepository<IResourceRelationRepository>();
            var referenceAtt = referenceProperty.GetCustomAttribute<ResourceReferenceAttribute>();

            // Get the value stored in the reference property
            var propertyValue = referenceProperty.GetValue(resource);
            var referencedResources = ((IEnumerable<IResource>)propertyValue).Cast<Resource>().ToList();

            // Check required attribute against empty collections
            if (referencedResources.Count == 0 && referenceAtt.IsRequired)
                throw new ValidationException($"Property {referenceProperty.Name} is flagged 'Required' and was empty!");

            // First delete references that are not used by ANY property of the same configuration
            var currentReferences = CurrentReferences(resource, referenceAtt);
            var deleted = relationTemplates.Where(m => currentReferences.All(cr => cr != context.ResolveReferencedResource(m))).ToList();
            foreach (var relation in deleted)
            {
                ClearOnTarget(context.ResolveReferencedResource(relation), resource, referenceAtt);
                relationRepo.Remove(relation.Entity);
            }

            // Now create new relations
            var created = referencedResources.Where(rr => relationTemplates.All(m => rr != context.ResolveReferencedResource(m))).ToList();
            foreach (var createdReference in created)
            {
                var relEntity = CreateRelationForProperty(context, relationRepo, referenceAtt);
                SetOnTarget(createdReference, resource, referenceAtt, relEntity);
                var referencedEntity = GetOrCreateEntity(context, createdReference);
                UpdateRelationEntity(entity, referencedEntity, relEntity, referenceAtt);
            }

            return created.Where(cr => cr.Id == 0);
        }

        /// <summary>
        /// Find all resources references by the instance with the same reference information
        /// </summary>
        private static ISet<IResource> CurrentReferences(Resource instance, ResourceReferenceAttribute referenceAtt)
        {
            // Get all references of this resource with the same relation information
            var currentReferences = (from property in ReferenceProperties(instance.GetType(), false)
                                     let att = property.GetCustomAttribute<ResourceReferenceAttribute>()
                                     where att.RelationType == referenceAtt.RelationType
                                           && att.Name == referenceAtt.Name
                                           && att.Role == referenceAtt.Role
                                     select property.GetValue(instance)).SelectMany(ExtractAllFromProperty);
            return new HashSet<IResource>(currentReferences);
        }

        /// <summary>
        /// Extract all resources from the property value for single and many references
        /// </summary>
        private static IEnumerable<IResource> ExtractAllFromProperty(object propertyValue)
        {
            // Check if it is a single reference
            var asResource = propertyValue as IResource;
            if (asResource != null)
                return [asResource];

            // Otherwise it must be a collection
            var asEnumerable = propertyValue as IEnumerable;
            if (asEnumerable != null)
                return asEnumerable.Cast<IResource>();

            return [];
        }

        /// <summary>
        ///  Find the property on the target type that acts as the back-link in the relationship
        /// </summary>
        /// <returns></returns>
        private static PropertyInfo FindBackLink(Resource target, Resource value, ResourceReferenceAttribute referenceAtt)
        {
            var propOnTarget = (from prop in ReferenceProperties(target.GetType(), false)
                                where IsInstanceOfReference(prop, value)
                                let backAtt = prop.GetCustomAttribute<ResourceReferenceAttribute>()
                                where backAtt.RelationType == referenceAtt.RelationType // Compare relation type
                                      && backAtt.Role != referenceAtt.Role // Validate inverse role
                                select prop).FirstOrDefault();
            return propOnTarget;
        }

        /// <summary>
        /// Check if a resource object is an instance of the property type
        /// </summary>
        private static bool IsInstanceOfReference(PropertyInfo property, Resource value)
        {
            var typeLimit = property.PropertyType;
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IReferences<>))
                typeLimit = property.PropertyType.GetGenericArguments()[0];
            return typeLimit.IsInstanceOfType(value);
        }

        /// <summary>
        /// Update backlink if possible
        /// </summary>
        private static void SetOnTarget(Resource target, Resource value, ResourceReferenceAttribute referenceAtt, ResourceRelationEntity relationEntity)
        {
            var prop = FindBackLink(target, value, referenceAtt);
            if (prop == null)
                return; // No back-link -> nothing to do
            // Update back-link property
            if (prop.PropertyType.IsInstanceOfType(value))
            {
                prop.SetValue(target, value);
            }
            else if (prop.GetValue(target) is IReferenceCollection collection && !collection.UnderlyingCollection.Contains(value))
                collection.UnderlyingCollection.Add(value);

            var backAttr = prop.GetCustomAttribute<ResourceReferenceAttribute>();
            UpdateRelationEntity(relationEntity, backAttr);
        }

        /// <summary>
        /// Remove the reference to the resource on a target object
        /// </summary>
        private void ClearOnTarget(Resource target, Resource value, ResourceReferenceAttribute referenceAtt)
        {
            var prop = FindBackLink(target, value, referenceAtt);
            if (prop == null)
                return; // No back-link -> nothing to do
            // Update property ONLY if it currently points to our resource
            var propValue = prop.GetValue(target);
            if (propValue == value)
            {
                prop.SetValue(target, null);
            }
            else
            {
                (propValue as IReferenceCollection)?.UnderlyingCollection.Remove(value);
            }
        }

        /// <summary>
        /// Get or create an entity for a resource instance
        /// </summary>
        private static ResourceEntity GetOrCreateEntity(ReferenceSaverContext context, Resource instance)
        {
            // First check if the context contains an entity for the instance
            if (context.EntityCache.ContainsKey(instance))
                return context.EntityCache[instance];

            ResourceEntity entity;
            if (instance.Id > 0)
            {
                entity = context.UnitOfWork.GetEntity<ResourceEntity>(instance);
            }
            else
            {
                entity = ResourceEntityAccessor.SaveToEntity(context.UnitOfWork, instance);
                context.ResourceLookup[entity] = instance;
            }

            // Get or create an entity for the instance
            return context.EntityCache[instance] = entity;
        }

        /// <summary>
        /// Create a <see cref="ResourceRelationEntity"/> entity for a property match
        /// </summary>
        private static ResourceRelationEntity CreateRelationForProperty(ReferenceSaverContext context, IResourceRelationRepository relationRepo, ResourceReferenceAttribute att)
        {
            var relationType = att.RelationType;
            var relEntity = relationRepo.Create((int)relationType);

            context.CreatedRelations.Add(relEntity);

            return relEntity;
        }

        /// <summary>
        /// Set <see cref="ResourceRelationEntity.SourceId"/> and <see cref="ResourceRelationEntity.TargetId"/> depending on the <see cref="ResourceReferenceRole"/>
        /// of the reference property
        /// </summary>
        private static void UpdateRelationEntity(ResourceEntity resource, ResourceEntity referencedResource, ResourceRelationEntity relEntity, ResourceReferenceAttribute att)
        {
            if (att.Role == ResourceReferenceRole.Source)
            {
                relEntity.SourceId = referencedResource.Id;
                relEntity.Source = referencedResource;
                relEntity.TargetId = resource.Id;
                relEntity.Target = resource;
                relEntity.SourceName = att.Name;
            }
            else
            {
                relEntity.Source = resource;
                relEntity.SourceId = resource.Id;
                relEntity.Target = referencedResource;
                relEntity.TargetId = referencedResource.Id;
                relEntity.TargetName = att.Name;
            }
        }

        /// <summary>
        /// Set <see cref="ResourceRelationEntity.SourceName"/> and <see cref="ResourceRelationEntity.TargetName"/> depending on the <see cref="ResourceReferenceRole"/>
        /// of the reference property
        /// </summary>
        private static void UpdateRelationEntity(ResourceRelationEntity relEntity, ResourceReferenceAttribute att)
        {
            if (att.Role == ResourceReferenceRole.Source)
                relEntity.SourceName = att.Name;
            else
                relEntity.TargetName = att.Name;
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
        /// Find the relation that matches the property by type and role
        /// </summary>
        private static IEnumerable<ResourceRelationAccessor> MatchingRelations(IEnumerable<ResourceRelationAccessor> relations, PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<ResourceReferenceAttribute>();
            var matches = from relation in relations
                          where relation.Role == attribute.Role
                          where relation.RelationType == attribute.RelationType
                          where relation.Name == attribute.Name
                          select relation;
            return matches;
        }

        /// <summary>
        /// Resolve instance matches for relations
        /// </summary>
        public static IEnumerable<Resource> InstanceProjection(IEnumerable<ResourceRelationAccessor> relations, PropertyInfo property,
            Func<ResourceRelationAccessor, Resource> instanceResolver)
        {
            return relations.Select(instanceResolver).Where(instance => IsInstanceOfReference(property, instance));
        }

        /// <summary>
        /// Filter relations by type compatibility
        /// </summary>
        /// <param name="relations"></param>
        /// <param name="property"></param>
        /// <param name="instanceResolver"></param>
        /// <returns></returns>
        public static IEnumerable<ResourceRelationAccessor> TypeFilter(IEnumerable<ResourceRelationAccessor> relations, PropertyInfo property,
            Func<ResourceRelationAccessor, Resource> instanceResolver)
        {
            return from relation in relations
                   let other = instanceResolver(relation)
                   where IsInstanceOfReference(property, other)
                   select relation;
        }

        /// <summary>
        /// Reusable method for resolver delegate
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        private Resource ResolveRefernceWithGraph(ResourceRelationAccessor relation) => Graph.Get(relation.ReferenceId);

        /// <summary>
        /// Context for the recursive save references structure
        /// </summary>
        private class ReferenceSaverContext
        {
            private readonly IResourceGraph _graph;

            public ReferenceSaverContext(IUnitOfWork unitOfWork, IResourceGraph graph, Resource initialInstance, ResourceEntity entity) : this(unitOfWork, graph)
            {
                EntityCache[initialInstance] = entity;
                if (initialInstance.Id == 0)
                    ResourceLookup[entity] = initialInstance;
            }

            public ReferenceSaverContext(IUnitOfWork uow, IResourceGraph graph)
            {
                _graph = graph;
                UnitOfWork = uow;
                EntityCache = new Dictionary<Resource, ResourceEntity>();
                ResourceLookup = new Dictionary<ResourceEntity, Resource>();
                CreatedRelations = new List<ResourceRelationEntity>();
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
            /// Reverse <see cref="EntityCache"/> to fetch instances by resource
            /// </summary>
            public IDictionary<ResourceEntity, Resource> ResourceLookup { get; }

            /// <summary>
            /// Accesor wrappers for relations that were created while saving references
            /// </summary>
            public IList<ResourceRelationEntity> CreatedRelations { get; }

            /// <summary>
            /// Resolve a referenced resource from a <paramref name="refAccessor"/>
            /// </summary>
            public Resource ResolveReferencedResource(ResourceRelationAccessor refAccessor)
            {
                return refAccessor.ReferenceId > 0 ? _graph.Get(refAccessor.ReferenceId) : ResourceLookup[refAccessor.ReferenceEntity];
            }
        }
    }
}
