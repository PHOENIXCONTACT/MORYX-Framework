// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
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
