using System;
using Marvin.AbstractionLayer.Resources;
using Marvin.Model;
using Marvin.Tools;
using Newtonsoft.Json;

namespace Marvin.Resources.Model
{
    /// <summary>
    /// Helper methods for typical resource creation tasks
    /// </summary>
    public static class EntityCreation
    {
        /// <summary>
        /// Creates a resource
        /// </summary>
        /// <param name="uow">The resource unit of work.</param>
        /// <param name="resource">Resource instance that shall be converted to an entity</param>
        /// <returns>The new created resource.</returns>
        public static ResourceEntity CreateResource(IUnitOfWork uow, IResource resource)
        {
            return CreateResource(uow, resource, null);
        }

        /// <summary>
        /// Creates a resource and set the relation to the parent.
        /// </summary>
        /// <param name="uow">The resource unit of work.</param>
        /// <param name="resource">Resource instance that shall be converted to an entity</param>
        /// <param name="parent">Ifa available, the parent of the resource.</param>
        /// <returns>The new created resource.</returns>
        public static ResourceEntity CreateResource(IUnitOfWork uow, IResource resource, ResourceEntity parent)
        {
            var repo = uow.GetRepository<IResourceEntityRepository>();

            var resourceEntity = repo.Create(resource.Name, resource.GetType().Name);
            if (string.IsNullOrWhiteSpace(resourceEntity.Name))
                resourceEntity.Name = resourceEntity.Type;
            resourceEntity.LocalIdentifier = resource.LocalIdentifier;
            resourceEntity.GlobalIdentifier = resource.GlobalIdentifier;
            resourceEntity.ExtensionData = JsonConvert.SerializeObject(resource, JsonSettings.Minimal);

            if (parent != null)
            {
                CreateRelation(uow, ResourceRelationType.ParentChild, parent, resourceEntity);
            }

            return resourceEntity;
        }

        /// <summary>
        /// Creates a relation between parent and child.
        /// </summary>
        /// <param name="uow">The resource unit of work.</param>
        /// <param name="relType">The relation type.</param>
        /// <param name="source">Resource entity parent.</param>
        /// <param name="target">resource entitiy child.</param>
        public static ResourceRelation CreateRelation(IUnitOfWork uow, ResourceRelationType relType, ResourceEntity source, ResourceEntity target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var relation = uow.GetRepository<IResourceRelationRepository>().Create((int)relType);
            relation.Source = source;
            relation.Target = target;

            return relation;
        }
    }
}