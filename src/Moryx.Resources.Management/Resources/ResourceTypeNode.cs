// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Serialization;

namespace Moryx.Resources.Management
{
    /// <summary>
    /// Class that represents a single resource type in the type tree
    /// </summary>
    internal class ResourceTypeNode : IResourceTypeNode
    {
        /// <inheritdoc />
        public string Name => ResourceType.ResourceType();

        private Type _resourceType;
        /// <inheritdoc />
        public Type ResourceType
        {
            get { return _resourceType; }
            set
            {
                _resourceType = value;
                PropertiesOfResourceType = _resourceType.GetProperties();
                ReferenceOverrides = (from prop in PropertiesOfResourceType
                                      let overrideAtt = prop.GetCustomAttribute<ReferenceOverrideAttribute>()
                                      where overrideAtt != null
                                      let targetType = typeof(IEnumerable<IResource>).IsAssignableFrom(prop.PropertyType)
                                        ? EntryConvert.ElementType(prop.PropertyType) : prop.PropertyType
                                      group targetType by overrideAtt.Source into g
                                      select new { g.Key, overrides = g.ToList() }).ToDictionary(v => v.Key, v => v.overrides);

                References = (from prop in PropertiesOfResourceType
                              let propType = prop.PropertyType
                              // Find all properties referencing a resource or a collection of resources
                              // Exclude read only properties, because they are simple type overrides of other references
                              where prop.CanWrite && Attribute.IsDefined(prop, typeof(ResourceReferenceAttribute))
                              select prop).ToList();
            }
        }

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

        public IEnumerable<PropertyInfo> References { get; private set; }

        public IEnumerable<PropertyInfo> PropertiesOfResourceType { get; private set; }

        public Dictionary<string, List<Type>> ReferenceOverrides { get; private set; }

        /// <inheritdoc />
        public override string ToString() => $"{ResourceType.Name}(Registered={IsRegistered})";
    }
}
