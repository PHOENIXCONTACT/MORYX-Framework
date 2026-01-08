// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.AbstractionLayer.Resources;
using Moryx.Model;
using Moryx.Model.Repositories;
using Moryx.Resources.Management.Model;
using Moryx.Serialization;
using Newtonsoft.Json;

namespace Moryx.Resources.Management
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
        public Resource Instantiate(IResourceTypeController typeController, IResourceGraph graph)
        {
            // This looks a little nasty, but it is the best way to prevent castle from
            // interfering with the resource relations
            Instance = typeController.Create(Type);

            // Resources need access to the creator
            Instance.Graph = graph;

            // Copy default properties
            Instance.Id = Id;
            Instance.Name = Name;
            Instance.Description = Description;

            // Copy extended data from json, or simply use JSON to provide defaults
            JsonConvert.PopulateObject(ExtensionData ?? "{}", Instance, JsonSettings.Minimal);

            return Instance;
        }

        /// <summary>
        /// Save the resource instance to a database entity
        /// </summary>
        public static async Task<ResourceEntity> SaveToEntity(IUnitOfWork uow, Resource instance)
        {
            // Create entity and populate from object
            var entity = await uow.FindEntityAsync<ResourceEntity>(instance);
            if (entity is null)
            {
                entity = await uow.CreateEntityAsync<ResourceEntity>(instance);
                entity.Type = instance.ResourceType();
            }

            // All those checks are necessary since EF change tracker does not recognize equal values as such
            if (entity.Name != instance.Name)
                entity.Name = instance.Name;
            if (entity.Description != instance.Description)
                entity.Description = instance.Description;
            var extensionData = JsonConvert.SerializeObject(instance, JsonSettings.Minimal);
            if (entity.ExtensionData != extensionData)
                entity.ExtensionData = extensionData;

            return entity;
        }

        /// <summary>
        /// Fetch all resources and their relations from the database 
        /// </summary>
        public static async Task<ICollection<ResourceEntityAccessor>> FetchResourceTemplates(IUnitOfWork uow)
        {
            var resourceRepo = uow.GetRepository<IResourceRepository>();

            var resources = await
                (from res in resourceRepo.Linq
                 where res.Deleted == null
                 select res)
                .ToListAsync();

            var resourceEntityAccessors = resources
                .Select(res =>
                    new ResourceEntityAccessor
                    {
                        Id = res.Id,
                        Type = res.Type,
                        Name = res.Name,
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
                    })
                .ToList();
            return resourceEntityAccessors;
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
        public ResourceRelationEntity Entity { get; set; }

        /// <summary>
        /// Optional name of the reference
        /// </summary>
        public string Name => Role == ResourceReferenceRole.Target ? Entity.TargetName : Entity.SourceName;

        /// <summary>
        /// Type of the reference relation
        /// </summary>
        public ResourceRelationType RelationType => (ResourceRelationType)Entity.RelationType;

        /// <summary>
        /// Id of the resource with the specified <see cref="Role"/> in the relation
        /// </summary>
        public long ReferenceId => Role == ResourceReferenceRole.Target ? Entity.TargetId : Entity.SourceId;

        /// <summary>
        /// Entity of the resource with the specified <see cref="Role"/> in the relation
        /// </summary>
        public ResourceEntity ReferenceEntity => Role == ResourceReferenceRole.Target ? Entity.Target : Entity.Source;

        /// <summary>
        /// Load all relations template of a resource entity
        /// </summary>
        public static async Task<ICollection<ResourceRelationAccessor>> FromEntity(IUnitOfWork uow, ResourceEntity entity)
        {
            if (entity.Id <= 0)
                return Array.Empty<ResourceRelationAccessor>();

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
        /// Load all relation templates from a queryable collection of <see cref="ResourceRelationEntity"/>
        /// </summary>
        public static ICollection<ResourceRelationAccessor> FromQueryable(IQueryable<ResourceRelationEntity> relations, ResourceEntity instance)
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
