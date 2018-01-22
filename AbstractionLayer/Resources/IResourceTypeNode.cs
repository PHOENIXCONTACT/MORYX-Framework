using System.Collections.Generic;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Represents a single node in the resource type tree
    /// </summary>
    public interface IResourceTypeNode
    {
        /// <summary>
        /// Name of the resource type
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Flag if this node can be instantiated
        /// </summary>
        bool Creatable { get; }

        /// <summary>
        /// Base type this node is derived from
        /// </summary>
        IResourceTypeNode BaseType { get; }
        
        /// <summary>
        /// Types derived from this node
        /// </summary>
        IEnumerable<IResourceTypeNode> DerivedTypes { get; }
    }
}