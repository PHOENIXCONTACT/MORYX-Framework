// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Facade for unintercepted access to resource objects, creation, modification and removal
    /// For lifecylce and memory reasons resources are not returned, instead access is granted through delegates
    /// </summary>
    // TODO: Move into IResourceManagement
    public interface IResourceModification : IResourceManagement
    {
        /// <summary>
        /// Create and initialize a resource
        /// </summary>
        long Create(Type resourceType, Action<Resource> initializer);

        /// <summary>
        /// Read data from a resource
        /// </summary>
        TResult Read<TResult>(long id, Func<Resource, TResult> accessor);
        
        /// <summary>
        /// Modify the resource. 
        /// </summary>
        /// <param name="id">Id of the resource</param>
        /// <param name="modifier">Modifier delegate, must return <value>true</value> in order to save changes</param>
        void Modify(long id, Func<Resource, bool> modifier);

        /// <summary>
        /// Create and initialize a resource
        /// </summary>
        bool Delete(long id);
    }
}