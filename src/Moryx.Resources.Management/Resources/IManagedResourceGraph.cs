// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management
{
    /// <summary>
    /// Additional interface for the resource graph used by <see cref="IResourceManager"/>
    /// </summary>
    internal interface IManagedResourceGraph : IResourceGraph
    {
        /// <summary>
        /// Add a resource to the graph
        /// </summary>
        Resource Add(Resource instance);

        /// <summary>
        /// Remove an instance from the graph
        /// </summary>
        bool Remove(Resource instance);

        /// <summary>
        /// Return the resource instance
        /// </summary>
        Resource GetResource(long id);

        /// <summary>
        /// Access the full graph
        /// </summary>
        ICollection<Resource> GetAll();

        /// <summary>
        /// TODO: Find a better way
        /// The ResourceGraph manages the resource instances, while the ResourceManager can save and destroy resources.
        /// The ResourceManager references the ResourceGraph. That's the reason why the ResourceGraph cannot reference the ResourceManager.
        /// In order to still be able to save and destroy resources, these delegates exist. 
        /// Events are also no alternatives, since there would be problems with the return values
        /// </summary>
        Action<Resource> SaveDelegate { get; set; }

        /// <summary>
        /// TODO: Find a better way
        /// </summary>
        Func<Resource, bool, bool> DestroyDelegate { get; set; }
    }
}
