using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Configuration;
using Marvin.Model;
using Marvin.Resources.Model;
using Marvin.Serialization;
using Marvin.Tools;
using Newtonsoft.Json;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Dedicated type to load a resources, its sources and targets from the database
    /// to instantiate it to a typed implementation of <see cref="IResource"/>.
    /// </summary>
    internal class ResourceEntityAccessor
    {
        /// <summary>
        /// Database key of the resource
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Type name of the resource object
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Name of the resource
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Local identifier of the resource within its context
        /// </summary>
        public string LocalIdentifier { get; set; }

        /// <summary>
        /// Global identifier of the resource
        /// </summary>
        public string GlobalIdentifier { get; set; }

        /// <summary>
        /// Description of the resource
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// JSON serialized custom properties of the resource
        /// </summary>
        public string ExtensionData { get; set; }

        /// <summary>
        /// All references of the resource in the database
        /// </summary>
        public ICollection<ResourceRelationAccessor> Relations { get; set; }

        /// <summary>
        /// Resource instance created from this template
        /// </summary>
        public Resource Instance { get; private set; }

        /// <summary>
        /// Instantiate the factory and return a populated resource object
        /// </summary>
        public Resource Instantiate(IResourceTypeController typeController, IResourceCreator creator)
        {
            // This looks a little nasty, but it is the best way to prevent castle from
            // interfering with the resource relations
            var resource = typeController.Create(Type);

            // Resources need access to the creator
            resource.Creator = creator;

            // Copy default properties
            resource.Id = Id;
            resource.Name = Name;
            resource.LocalIdentifier = LocalIdentifier;
            resource.GlobalIdentifier = GlobalIdentifier;
            resource.Description = Description;

            // Copy extended data from json
            if (ExtensionData != null)
                JsonConvert.PopulateObject(ExtensionData, resource, JsonSettings.Minimal);

            // Read everything else from defaults
            ValueProviderExecutor.Execute(resource, new ValueProviderExecutorSettings()
                                                        .AddFilter(new DataMemberAttributeValueProviderFilter(false))
                                                        .AddDefaultValueProvider());

            return Instance = resource;
        }

        /// <summary>
        /// Save the resource instance to a database entity
        /// </summary>
        public static ResourceEntity SaveToEntity(IUnitOfWork uow, Resource instance)
        {
            // Create entity and populate from object
            var entity = uow.GetEntity<ResourceEntity>(instance);
            if (entity.Id == 0)
                entity.Type = instance.ResourceType();

            entity.Name = instance.Name;
            entity.Description = instance.Description;
            entity.LocalIdentifier = instance.LocalIdentifier;
            entity.GlobalIdentifier = instance.GlobalIdentifier;
            entity.ExtensionData = JsonConvert.SerializeObject(instance, JsonSettings.Minimal);

            return entity;
        }

        /// <summary>
        /// Fetch all resources and their relations from the database 
        /// </summary>
        public static ICollection<ResourceEntityAccessor> FetchResourceTemplates(IUnitOfWork uow)
        {
            var resourceRepo = uow.GetRepository<IResourceEntityRepository>();

            var resources = (from res in resourceRepo.Linq
                             where res.Deleted == null
                             select new ResourceEntityAccessor
                             {
                                 Id = res.Id,
                                 Type = res.Type,
                                 Name = res.Name,
                                 LocalIdentifier = res.LocalIdentifier,
                                 GlobalIdentifier = res.GlobalIdentifier,
                                 Description = res.Description,
                                 ExtensionData = res.ExtensionData,
                                 Relations = (from target in res.Targets
                                              where target.Target.Deleted == null
                                              // Attention: This is Copy&Paste because of LinQ limitations
                                              select new ResourceRelationAccessor
                                              {
                                                  Entity = target,
                                                  Role = ResourceReferenceRole.Target,
                                              }).Concat(
                                               from source in res.Sources
                                               where source.Source.Deleted == null
                                               // Attention: This is Copy&Paste because of LinQ limitations
                                               select new ResourceRelationAccessor
                                               {
                                                   Entity = source,
                                                   Role = ResourceReferenceRole.Source,
                                               }).ToList()
                             }).ToList();
            return resources;
        }

        /// <summary>
        /// Return Id, name and type of the resource
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Id}:{Name} ({Type})";
        }
    }

    /// <summary>
    /// Unified representation of <see cref="ResourceEntity.Sources"/> and <see cref="ResourceEntity.Targets"/>
    /// </summary>
    internal class ResourceRelationAccessor
    {
        /// <summary>
        /// Role of the referenced resource in the relation
        /// </summary>
        public ResourceReferenceRole Role { get; set; }

        /// <summary>
        /// Relation entity represented by this template
        /// </summary>
        public ResourceRelation Entity { get; set; }

        /// <summary>
        /// Optional name of the reference
        /// </summary>
        public string Name => Role == ResourceReferenceRole.Target ? Entity.TargetName : Entity.SourceName;

        /// <summary>
        /// Type of the reference relation
        /// </summary>
        public ResourceRelationType RelationType => (ResourceRelationType)Entity.RelationType;

        /// <summary>
        /// Id of the referenced resource
        /// </summary>
        public long ReferenceId => Role == ResourceReferenceRole.Target ? Entity.TargetId : Entity.SourceId;

        /// <summary>
        /// Load all relations template of a resource entity
        /// </summary>
        public static ICollection<ResourceRelationAccessor> FromEntity(IUnitOfWork uow, ResourceEntity entity)
        {
            if (entity.Id <= 0)
                return new ResourceRelationAccessor[0];

            var relationRepo = uow.GetRepository<IResourceRelationRepository>();
            var result = (from target in relationRepo.Linq
                          where target.Target.Deleted == null && target.SourceId == entity.Id
                          // Attention: This is Copy&Paste because of LinQ limitations
                          select new ResourceRelationAccessor
                          {
                              Entity = target,
                              Role = ResourceReferenceRole.Target,
                          }).Concat(
                          from source in relationRepo.Linq
                          where source.Source.Deleted == null && source.TargetId == entity.Id
                          // Attention: This is Copy&Paste because of LinQ limitations
                          select new ResourceRelationAccessor
                          {
                              Entity = source,
                              Role = ResourceReferenceRole.Source,
                          }).ToList();
            return result;
        }

        /// <summary>
        /// Load all relation templates from a queryable collection of <see cref="ResourceRelation"/>
        /// </summary>
        public static ICollection<ResourceRelationAccessor> FromQueryable(IQueryable<ResourceRelation> relations, ResourceEntity instance)
        {
            var result = (from target in relations
                          where target.Target.Deleted == null && target.Source == instance
                          // Attention: This is Copy&Paste because of LinQ limitations
                          select new ResourceRelationAccessor
                          {
                              Entity = target,
                              Role = ResourceReferenceRole.Target,
                          }).Concat(
                          from source in relations
                          where source.Source.Deleted == null && source.Target == instance
                          // Attention: This is Copy&Paste because of LinQ limitations
                          select new ResourceRelationAccessor
                          {
                              Entity = source,
                              Role = ResourceReferenceRole.Source,
                          }).ToList();
            return result;
        }
    }
}