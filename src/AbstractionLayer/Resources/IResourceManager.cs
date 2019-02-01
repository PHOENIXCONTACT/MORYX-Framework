using System;
using System.Collections.Generic;
using Marvin.Modules;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Major component managing the resource graph
    /// TODO: Remove inherited IResourceManagement. Facade uses the ResourceGraph instead
    /// </summary>
    public interface IResourceManager : IInitializablePlugin, IResourceManagement, IResourceCreator
    {
        /// <summary>
        /// Create a new resource instance but DO NOT save it
        /// </summary>
        [Obsolete("Use Instantiate from ResourceCreator instead")]
        Resource Create(string type);

        /// <summary>
        /// Get the resource with this id
        /// </summary>
        [Obsolete("Use Get from ResourceGraph instead")]
        Resource Get(long id);

        /// <summary>
        /// Returns all resources without a parent
        /// </summary>
        [Obsolete("Use GetResources with predicate from ResourceGraph instead")]
        IReadOnlyList<Resource> GetRoots();

        /// <summary>
        /// Executes the intializer on this creator
        /// </summary>
        void ExecuteInitializer(IResourceInitializer initializer);

        /// <summary>
        /// Write changes on this object to the database
        /// </summary>
        /// <param name="resource"></param>
        void Save(Resource resource);

        /// <summary>
        /// Start the resource with this id
        /// </summary>
        bool Start(Resource resource);

        /// <summary>
        /// Stop the execution of the resource. It will no lange handle incoming messages
        /// </summary>
        bool Stop(Resource resource);
    }
}