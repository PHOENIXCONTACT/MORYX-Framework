using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Additional interface for the resource graph used by <see cref="IResourceManager"/>
    /// </summary>
    internal interface IManagedResourceGraph : IResourceGraph
    {
        /// <summary>
        /// Add a resource to the graph
        /// </summary>
        ResourceWrapper Add(Resource instance);

        /// <summary>
        /// Remove an instance from the graph
        /// </summary>
        bool Remove(Resource instance);

        /// <summary>
        /// Return the wrapper of a resource instance
        /// </summary>
        ResourceWrapper GetWrapper(long id);

        /// <summary>
        /// Access the full graph
        /// </summary>
        ICollection<ResourceWrapper> GetAll();

        /// <summary>
        /// TODO: Find a better way
        /// </summary>
        Action<Resource> SaveDelegate { get; set; }

        /// <summary>
        /// TODO: Find a better way
        /// </summary>
        Func<Resource, bool, bool> DestroyDelegate { get; set; }
    }
}