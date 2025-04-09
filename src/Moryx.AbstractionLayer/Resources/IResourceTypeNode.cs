// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.AbstractionLayer.Resources
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
        /// Type definition wrapped by this object
        /// </summary>
        Type ResourceType { get; set; }

        /// <summary>
        /// Flag if this node can be instantiated
        /// </summary>
        bool Creatable { get; }

        /// <summary>
        /// Methods on resources used to construct new instances
        /// </summary>
        MethodInfo[] Constructors { get; }

        /// <summary>
        /// Base type this node is derived from
        /// </summary>
        IResourceTypeNode BaseType { get; }
        
        /// <summary>
        /// Types derived from this node
        /// </summary>
        IEnumerable<IResourceTypeNode> DerivedTypes { get; }

        IEnumerable<PropertyInfo> References { get;  }

        IEnumerable<PropertyInfo> PropertiesOfResourceType { get;  }

        Dictionary<string, List<Type>> ReferenceOverrides { get;  }


    }
}
