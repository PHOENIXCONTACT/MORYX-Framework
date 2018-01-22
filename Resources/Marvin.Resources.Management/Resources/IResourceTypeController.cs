using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer.Resources;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Central component responsible for reading the type tree and building/managing proxies
    /// </summary>
    internal interface IResourceTypeController : IModulePlugin
    {
        /// <summary>
        /// Types derived from <see cref="Resource"/> and their derived types
        /// </summary>
        IEnumerable<ResourceTypeNode> RootTypes { get; }

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