// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Reflection;
using Moryx.AbstractionLayer.Resources;
using Moryx.Model.Repositories;
using Moryx.Resources.Model;

namespace Moryx.Resources.Management
{
    /// <summary>
    /// Helper class responsible for linking the flat list of resources
    /// into a directed graph
    /// </summary>
    internal interface IResourceLinker
    {
        /// <summary>
        /// Saves all roots to the database
        /// </summary>
        IReadOnlyList<Resource> SaveRoots(IUnitOfWork uow, IReadOnlyList<Resource> instances);

        /// <summary>
        /// Link all reference properties of an instance using the relation information
        /// and complete resource collection
        /// </summary>
        /// <returns>List of reference collections configured for autosave</returns>
        void LinkReferences(Resource resource, ICollection<ResourceRelationAccessor> relations);

        /// <summary>
        /// Save all references of a resource to the database. Use the creator callback to save new instances discovered
        /// in relations on the fly.
        /// </summary>
        /// <returns>Found new instances</returns>
        IReadOnlyList<Resource> SaveReferences(IUnitOfWork uow, Resource instance, ResourceEntity entity, Dictionary<Resource, ResourceEntity> partsDict = null);

        /// <summary>
        /// Save changes to a single collection
        /// </summary>
        /// <returns>Found new instances</returns>
        IReadOnlyList<Resource> SaveSingleCollection(IUnitOfWork uow, Resource instance, PropertyInfo property);

        /// <summary>
        /// Remove all links to the deleted instance on the reference
        /// </summary>
        /// <param name="deletedInstance">Resource that is being deleted</param>
        /// <param name="reference">Resource referencing the deleted instance</param>
        void RemoveLinking(IResource deletedInstance, IResource reference);
    }
}
