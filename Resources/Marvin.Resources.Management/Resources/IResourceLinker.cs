using System.Collections.Generic;
using System.Reflection;
using Marvin.AbstractionLayer.Resources;
using Marvin.Model;
using Marvin.Resources.Model;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Helper class responsible for linking the flat list of resources
    /// into a directed graph
    /// </summary>
    internal interface IResourceLinker
    {
        /// <summary>
        /// Set all reference collections on the new instance
        /// </summary>
        void SetReferenceCollections(Resource instance);

        /// <summary>
        /// Get all autosave collections on a resource instance. This is necessary to register
        /// to the <see cref="IReferenceCollection.CollectionChanged"/> event.
        /// </summary>
        ICollection<IReferenceCollection> GetAutoSaveCollections(Resource instance);

        /// <summary>
        /// Link all reference properties of an instance using the relation information
        /// and complete resource collection
        /// TODO: Remove parent from API
        /// </summary>
        /// <returns>List of reference collections configured for autosave</returns>
        void LinkReferences(Resource resource, ICollection<ResourceRelationTemplate> relations, ResourceManager parent);

        /// <summary>
        /// Save all references of a resource to the database. Use the creator callback to save new instances discovered
        /// in relations on the fly.
        /// TODO: Remove parent from API
        /// </summary>
        void SaveReferences(IUnitOfWork uow, Resource instance, ResourceEntity entity, ResourceManager parent);

        /// <summary>
        /// Save changes to a single collection
        /// TODO: Remove parent from API
        /// </summary>
        void SaveSingleCollection(IUnitOfWork uow, Resource instance, PropertyInfo property, ResourceManager parent);

        /// <summary>
        /// Remove all links to the deleted instance on the reference
        /// </summary>
        /// <param name="deletedInstance">Resource that is being deleted</param>
        /// <param name="reference">Resource referencing the deleted instance</param>
        void RemoveLinking(IResource deletedInstance, IResource reference);

    }
}