using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Configuration;
using Marvin.Model;
using Marvin.Resources.Model;
using Marvin.Runtime.Configuration;
using Marvin.Tools;
using Newtonsoft.Json;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Dedicated type to load a resources, its sources and targets from the database
    /// to instantiate it to a typed implementation of <see cref="IResource"/>.
    /// </summary>
    internal class ResourceCreationTemplate
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
        /// JSON serialized custom properties of the resource
        /// </summary>
        public string ExtensionData { get; set; }

        /// <summary>
        /// All references of the resource in the database
        /// </summary>
        public ICollection<ResourceRelationTemplate> Relations { get; set; }

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

            // Copy extended data from json
            if (ExtensionData != null)
                JsonConvert.PopulateObject(ExtensionData, resource, JsonSettings.Minimal);

            // Read everything else from defaults
            ValueProvider.FillProperties(resource, PropertyFilter, DefaultValueProvider.CheckPropertyForDefault);

            return Instance = resource;
        }

        /// <summary>
        /// Value provider that filters properties that shall be ignored by the provider.
        /// It will flag all properties without <see cref="DataMember"/> attribute as handled
        /// to break the call in <see cref="ValueProvider"/>
        /// </summary>
        private static bool PropertyFilter(object target, PropertyInfo property)
        {
            return property.GetCustomAttribute<DataMemberAttribute>() == null;
        }

        /// <summary>
        /// Fetch all resources and their relations from the database 
        /// </summary>
        public static ICollection<ResourceCreationTemplate> FetchResourceTemplates(IUnitOfWork uow)
        {
            var resourceRepo = uow.GetRepository<IResourceEntityRepository>();

            var resources = (from res in resourceRepo.Linq
                             where res.Deleted == null
                             select new ResourceCreationTemplate
                             {
                                 Id = res.Id,
                                 Type = res.Type,
                                 Name = res.Name,
                                 LocalIdentifier = res.LocalIdentifier,
                                 GlobalIdentifier = res.GlobalIdentifier,
                                 ExtensionData = res.ExtensionData,
                                 Relations = (from target in res.Targets
                                              where target.Target.Deleted == null
                                              // Attention: This is Copy&Paste because of LinQ limitations
                                              select new ResourceRelationTemplate
                                              {
                                                  Name = target.RelationName,
                                                  Role = ResourceReferenceRole.Target,
                                                  RelationType = (ResourceRelationType)target.RelationType,
                                                  ReferenceId = target.TargetId
                                              }).Concat(
                                               from source in res.Sources
                                               where source.Source.Deleted == null
                                               // Attention: This is Copy&Paste because of LinQ limitations
                                               select new ResourceRelationTemplate
                                               {
                                                   Name = source.RelationName,
                                                   Role = ResourceReferenceRole.Source,
                                                   RelationType = (ResourceRelationType)source.RelationType,
                                                   ReferenceId = source.SourceId
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
    internal class ResourceRelationTemplate
    {
        /// <summary>
        /// Role of the referenced resource in the relation
        /// </summary>
        public ResourceReferenceRole Role { get; set; }

        /// <summary>
        /// Type of the reference relation
        /// </summary>
        public ResourceRelationType RelationType { get; set; }

        /// <summary>
        /// Optional name of the reference
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id of the referenced resource
        /// </summary>
        public long ReferenceId { get; set; }

        /// <summary>
        /// Relation entity represented by this template
        /// </summary>
        public ResourceRelation Entity { get; set; }

        /// <summary>
        /// Load all relations template of a resource entity
        /// </summary>
        public static ICollection<ResourceRelationTemplate> FromEntity(IUnitOfWork uow, ResourceEntity entity)
        {
            if (entity.Id <= 0)
                return new ResourceRelationTemplate[0];

            var relationRepo = uow.GetRepository<IResourceRelationRepository>();
            var relations = (from target in relationRepo.Linq
                             where target.Target.Deleted == null && target.SourceId == entity.Id
                             // Attention: This is Copy&Paste because of LinQ limitations
                             select new ResourceRelationTemplate
                             {
                                 Entity = target,
                                 Name = target.RelationName,
                                 Role = ResourceReferenceRole.Target,
                                 RelationType = (ResourceRelationType)target.RelationType,
                                 ReferenceId = target.TargetId
                             }).Concat(
                             from source in relationRepo.Linq
                             where source.Source.Deleted == null && source.TargetId == entity.Id
                             // Attention: This is Copy&Paste because of LinQ limitations
                             select new ResourceRelationTemplate
                             {
                                 Entity = source,
                                 Name = source.RelationName,
                                 Role = ResourceReferenceRole.Source,
                                 RelationType = (ResourceRelationType)source.RelationType,
                                 ReferenceId = source.SourceId
                             }).ToList();
            return relations;
        }
    }
}