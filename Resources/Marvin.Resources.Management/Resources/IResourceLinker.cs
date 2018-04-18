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
        /// </summary>
        /// <returns>List of reference collections configured for autosave</returns>
        void LinkReferences(Resource resource, ICollection<ResourceRelationAccessor> relations, IDictionary<long, ResourceWrapper> allResources);

        /// <summary>
        /// Save all references of a resource to the database. Use the creator callback to save new instances discovered
        /// in relations on the fly.
        /// </summary>
        /// <returns>Found new instances</returns>
        IEnumerable<Resource> SaveReferences(IUnitOfWork uow, Resource instance, ResourceEntity entity);

        /// <summary>
        /// Save changes to a single collection
        /// </summary>
        /// <returns>Found new instances</returns>
        IEnumerable<Resource> SaveSingleCollection(IUnitOfWork uow, Resource instance, PropertyInfo property);

        /// <summary>
        /// Remove all links to the deleted instance on the reference
        /// </summary>
        /// <param name="deletedInstance">Resource that is being deleted</param>
        /// <param name="reference">Resource referencing the deleted instance</param>
        void RemoveLinking(IResource deletedInstance, IResource reference);
    }
}