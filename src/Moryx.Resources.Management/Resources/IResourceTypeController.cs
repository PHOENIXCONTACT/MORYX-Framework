// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.AbstractionLayer.Resources;
using Moryx.Modules;

namespace Moryx.Resources.Management
{
    /// <summary>
    /// Central component responsible for reading the type tree and building/managing proxies
    /// </summary>
    internal interface IResourceTypeController : IPlugin
    {
        /// <summary>
        /// Type tree starting at <see cref="Resource"/>
        /// </summary>
        ResourceTypeNode RootType { get; }

        /// <summary>
        /// Get the proxy for this resource
        /// </summary>
        IResource GetProxy(Resource instance);

        /// <summary>
        /// Create a resource instance
        /// </summary>
        Resource Create(string type);

        /// <summary>
        /// Destroy an instance and release it for garbage collection
        /// </summary>
        void Destroy(Resource instance);

        /// <summary>
        /// Find all types that implement the given type constraint
        /// </summary>
        IEnumerable<ResourceTypeNode> SupportedTypes(Type constraint);

        /// <summary>
        /// Find all resource type trees that implement the given type constraints
        /// </summary>
        IEnumerable<ResourceTypeNode> SupportedTypes(ICollection<Type> constraints);
    }
}
