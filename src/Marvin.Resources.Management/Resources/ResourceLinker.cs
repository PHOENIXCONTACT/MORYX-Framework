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
using static Marvin.Resources.Management.ResourceReferenceTools;

namespace Marvin.Resources.Management
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
            var context = new ReferenceSaverContext(uow);
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

            foreach (var property in referenceProperties)
            {
                // Link a list of resources
                if (typeof(IEnumerable<IResource>).IsAssignableFrom(property.PropertyType))
                {
                    // Read attribute and get the ReferenceCollection
                    var value = (IReferenceCollection)property.GetValue(resource);
                    var elemType = property.PropertyType.GetGenericArguments()[0];

                    var matches = MatchingRelations(relations, property);
                    var resources = matches.Select(m => Graph.Get(m.ReferenceId))
                        .Where(elemType.IsInstanceOfType)
                        .OrderBy(r => r.Name);

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
                                          let reference = Graph.Get(match.ReferenceId)
                                          where propertyType.IsInstanceOfType(reference)
                                          select reference).ToArray();
                    if (referenceMatch.Length == 1)
                        property.SetValue(resource, referenceMatch[0]);
                    else if (referenceMatch.Length > 1)
                        Logger.LogEntry(LogLevel.Warning, "Inconclusive relation: Can not assign property {0} on {1}:{2} from [{3}]. Too many matches!", property.Name, resource.Id, resource.Name, string.Join(",", matches.Select(m => m.ReferenceId)));
                    else if (matches.Any(m => referenceProperties.All(p => !PropertyMatchesRelation(p, m, Graph.Get(m.ReferenceId)))))
                        Logger.LogEntry(LogLevel.Warning, "Incompatible relation: Resources from [{0}] with relation type {1} can not be assigned to a property on {2}:{3}.", string.Join(",", matches.Select(m => m.ReferenceId)), matches[0].RelationType, resource.Id, resource.Name);
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
        public IReadOnlyList<Resource> SaveReferences(IUnitOfWork uow, Resource instance, ResourceEntity entity)
        {
            var context = new ReferenceSaverContext(uow, instance, entity);
            SaveReferences(context, instance);
            return context.EntityCache.Keys.Where(i => i.Id == 0).ToList();
        }

        private void SaveReferences(ReferenceSaverContext context, Resource instance)
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
        public IReadOnlyList<Resource> SaveSingleCollection(IUnitOfWork uow, Resource instance, PropertyInfo property)
        {
            var entity = uow.GetEntity<ResourceEntity>(instance);
            var relations = ResourceRelationAccessor.FromEntity(uow, entity);
            var matches = MatchingRelations(relations, property);

            var context = new ReferenceSaverContext(uow, instance, entity);
            var created = UpdateCollectionReference(context, entity, instance, property, matches);

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

            // Check if there is a relation that represents this reference
            if (referencedResource != null && matches.Any(m => m.ReferenceId == referencedResource.Id))
                return null;

            var referenceAtt = referenceProperty.GetCustomAttribute<ResourceReferenceAttribute>();
            // Get all references of this resource with the same relation information
            var currentReferences = (from property in ReferenceProperties(resource.GetType(), false)
                                     let att = property.GetCustomAttribute<ResourceReferenceAttribute>()
                                     where att.RelationType == referenceAtt.RelationType
                                        && att.Name == referenceAtt.Name
                                        && att.Role == referenceAtt.Role
                                     select property.GetValue(resource)).Distinct().OfType<Resource>().ToList();
            // Try to find a match that is not used in any reference but has no name yet or the same name
            var relMatch = (from match in matches
                            where currentReferences.All(cr => cr.Id != match.ReferenceId)
                            select match).FirstOrDefault();
            var relEntity = relMatch?.Entity;
            if (relEntity == null && referencedResource != null)
            {
                // Create a new relation
                relEntity = CreateRelationForProperty(context, relationRepo, referenceAtt);
                SetOnTarget(referencedResource, resource, referenceAtt);
            }
            else if (relEntity != null && referencedResource == null)
            {
                // Delete a relation, that no longer exists
                ClearOnTarget(relMatch.ReferenceId, resource, referenceAtt);
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
                ClearOnTarget(relMatch.ReferenceId, resource, referenceAtt);
                SetOnTarget(referencedResource, resource, referenceAtt);
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

            // First delete references that no longer exist
            var deleted = relationTemplates.Where(m => referencedResources.All(r => r.Id != m.ReferenceId)).ToList();
            foreach (var relation in deleted)
            {
                ClearOnTarget(relation.ReferenceId, resource, referenceAtt);
                relationRepo.Remove(relation.Entity);
            }

            // Now create new relations
            var created = referencedResources.Where(r => relationTemplates.All(m => m.ReferenceId != r.Id)).ToList();
            foreach (var createdReference in created)
            {
                SetOnTarget(createdReference, resource, referenceAtt);
                var relEntity = CreateRelationForProperty(context, relationRepo, referenceAtt);
                var referencedEntity = GetOrCreateEntity(context, createdReference);
                UpdateRelationEntity(entity, referencedEntity, relEntity, referenceAtt);
            }

            return created.Where(cr => cr.Id == 0);
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
                                where backAtt.Name == referenceAtt.Name // Compare name
                                      && backAtt.RelationType == referenceAtt.RelationType // Compare relation type
                                      && backAtt.Role != referenceAtt.Role // Validate inverse role
                                select prop).FirstOrDefault();
            return propOnTarget;
        }

        /// <summary>
        /// Check if a resource object is an instance of the property type
        /// </summary>
        private static bool IsInstanceOfReference(PropertyInfo property, Resource value)
        {
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IReferences<>))
                return property.PropertyType.GetGenericArguments()[0].IsInstanceOfType(value);
            return property.PropertyType.IsInstanceOfType(value);
        }

        /// <summary>
        /// Update backlink if possible
        /// </summary>
        private static void SetOnTarget(Resource target, Resource value, ResourceReferenceAttribute referenceAtt)
        {
            var prop = FindBackLink(target, value, referenceAtt);
            if (prop == null)
                return; // No back-link -> nothing to do
            // Update back-link property
            var propValue = prop.GetValue(target);
            if (prop.PropertyType.IsInstanceOfType(value))
            {
                prop.SetValue(target, value);
            }
            else if (propValue is IReferenceCollection)
            {
                ((IReferenceCollection)propValue).UnderlyingCollection.Add(value);
            }
        }

        /// <summary>
        /// Remove the reference to the resource on a target object
        /// </summary>
        private void ClearOnTarget(long oldReference, Resource value, ResourceReferenceAttribute referenceAtt)
        {
            var target = Graph.Get(oldReference);
            var prop = FindBackLink(target, value, referenceAtt);
            if (prop == null)
                return; // No back-link -> nothing to do
            // Update property ONLY if it currently points to our resource
            var propValue = prop.GetValue(target);
            if (prop.PropertyType.IsInstanceOfType(value) && propValue == value)
            {
                prop.SetValue(target, null);
            }
            else if (propValue is IReferenceCollection)
            {
                ((IReferenceCollection)propValue).UnderlyingCollection.Remove(value);
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
                relEntity.SourceName = att.Name;
            }
            else
            {
                relEntity.Source = resource;
                relEntity.Target = referencedResource;
                relEntity.TargetName = att.Name;
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
        /// Find the relation that matches the property by type and role
        /// </summary>
        private static IReadOnlyList<ResourceRelationAccessor> MatchingRelations(IEnumerable<ResourceRelationAccessor> relations, PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<ResourceReferenceAttribute>();
            var matches = (from relation in relations
                           where attribute.Role == relation.Role
                           where attribute.RelationType == relation.RelationType // Typed relation without name or matching name
                              && attribute.Name == relation.Name
                           select relation).ToArray();
            return matches;
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