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
        public void SetReferenceCollections(Resource instance)
        {
            var resourceType = instance.GetType();
            // Iterate all references and provide reference collections
            var overrides = new Dictionary<PropertyInfo, ReferenceOverrideAttribute>();
            foreach (var property in CollectionReferenceProperties(resourceType))
            {
                var attribute = property.GetCustomAttribute<ReferenceOverrideAttribute>();
                if (attribute == null)
                {
                    // Create collection and set on property
                    var value = CreateCollection(instance, property, property, new List<IResource>());
                    property.SetValue(instance, value);
                }
                else
                {
                    // Save overrides for later
                    overrides[property] = attribute;
                }
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
                var value = CreateCollection(instance, property, target, sourceCollection.UnderlyingCollection);
                property.SetValue(instance, value);
            }
        }

        /// <summary>
        /// Create a <see cref="ReferenceCollection{TResource}"/> instance
        /// </summary>
        private static IReferenceCollection CreateCollection(Resource instance, PropertyInfo property, PropertyInfo targetProperty, ICollection<IResource> underlyingCollection)
        {
            var propertyType = property.PropertyType;
            var referenceType = propertyType.GetGenericArguments()[0]; // Type of resource from ICollection<ResourceType>
            var collectionType = typeof(ReferenceCollection<>).MakeGenericType(referenceType); // Make generic ReferenceCollection
            var value = (IReferenceCollection)Activator.CreateInstance(collectionType, instance, targetProperty, underlyingCollection);
            return value;
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
        public void LinkReferences(Resource resource, ICollection<ResourceRelationTemplate> relations, ResourceManager parent)
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
                    var resources = matches.Select(m => parent.Get(m.ReferenceId))
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
                        let reference = parent.Get(match.ReferenceId)
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
        public void SaveReferences(IUnitOfWork uow, Resource instance, ResourceEntity entity, ResourceManager parent)
        {
            var relations = ResourceRelationTemplate.FromEntity(uow, entity);
            foreach (var referenceProperty in ReferenceProperties(instance.GetType(), false))
            {
                var matches = MatchingRelations(relations, referenceProperty);
                if (typeof(IEnumerable<IResource>).IsAssignableFrom(referenceProperty.PropertyType))
                {
                    // Save a collection reference
                    UpdateCollectionReference(uow, parent, entity, instance, referenceProperty, matches);
                }
                else
                {
                    // Save a single reference
                    UpdateSingleReference(uow, parent, entity, instance, referenceProperty, matches);
                }
            }
        }

        /// <inheritdoc />
        public void SaveSingleCollection(IUnitOfWork uow, Resource instance, PropertyInfo property, ResourceManager parent)
        {
            var entity = uow.GetEntity<ResourceEntity>(instance);
            var relations = ResourceRelationTemplate.FromEntity(uow, entity);
            var matches = MatchingRelations(relations, property);
            UpdateCollectionReference(uow, parent, entity, instance, property, matches);
        }

        /// <summary>
        /// Make sure our resource-relation graph in the database is synced to the resource object graph. This method
        /// updates single references like in the example below
        /// </summary>
        /// <example>
        /// [ResourceReference(ResourceRelationType.TransportRoute, ResourceReferenceRole.Source)]
        /// public Resource FriendResource { get; set; }
        /// </example>
        private void UpdateSingleReference(IUnitOfWork uow, ResourceManager parent, ResourceEntity entity, Resource resource, PropertyInfo referenceProperty, IReadOnlyList<ResourceRelationTemplate> matches)
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
                             where propertyType.IsInstanceOfType(parent.Get(match.ReferenceId))
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
            var referencedEntity = referencedResource.Id > 0 ? uow.GetEntity<ResourceEntity>(referencedResource) : parent.SaveResource(uow, referencedResource);
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
        private void UpdateCollectionReference(IUnitOfWork uow, ResourceManager parent, ResourceEntity entity, Resource resource, PropertyInfo referenceProperty, IReadOnlyList<ResourceRelationTemplate> relationTemplates)
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
                var referencedEntity = createdReference.Id > 0 ? uow.GetEntity<ResourceEntity>(createdReference) : parent.SaveResource(uow, createdReference);
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
    }
}