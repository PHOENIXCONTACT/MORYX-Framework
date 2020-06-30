// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Reflection;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Class that represents a single resource type in the type tree
    /// </summary>
    internal class ResourceTypeNode : IResourceTypeNode
    {
        /// <inheritdoc />
        public string Name => ResourceType.ResourceType();
        
        /// <inheritdoc />
        public Type ResourceType { get; set; }

        /// <inheritdoc />
        public bool Creatable => !ResourceType.IsAbstract;

        /// <inheritdoc />
        public MethodInfo[] Constructors { get; set; }

        /// <summary>
        /// Flag if this type is registered in the <see cref="IContainer"/> instance
        /// </summary>
        public bool IsRegistered { get; set; }

        /// <summary>
        /// Base type of this resource for the full type tree
        /// </summary>
        public ResourceTypeNode BaseType { get; set; }

        /// <summary>
        /// Derived types of this resource
        /// </summary>
        public ResourceTypeNode[] DerivedTypes { get; set; }

        /// <inheritdoc />
        IResourceTypeNode IResourceTypeNode.BaseType => BaseType;

        /// <inheritdoc />
        IEnumerable<IResourceTypeNode> IResourceTypeNode.DerivedTypes => DerivedTypes;

        /// <inheritdoc />
        public override string ToString() => $"{ResourceType.Name}(Registered={IsRegistered})";
    }
}
