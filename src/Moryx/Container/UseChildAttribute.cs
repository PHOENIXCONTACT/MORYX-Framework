// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Container
{
    /// <summary>
    /// Attribute used to decorate dependencies which are part of larger container and shall be resolved by name
    /// from this container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class UseChildAttribute : Attribute
    {
        /// <summary>
        /// Name of the child to resolve
        /// </summary>
        public string ChildName { get; }

        /// <summary>
        /// Optional name of the model, if more than one is available
        /// </summary>
        public string ParentName { get; }

        /// <summary>
        /// Request a child instance with the same name custom tailored for this component
        /// </summary>
        public UseChildAttribute()
        {
            ChildName = string.Empty;
        }

        /// <summary>
        /// Request a named child of the parent component
        /// </summary>
        /// <param name="childName">Name of the child to resolve</param>
        public UseChildAttribute(string childName)
        {
            ChildName = childName;
        }

        /// <summary>
        /// Fetch child of a named parent, of there might be more than one
        /// </summary>
        /// <param name="childName">Name of the child to resolve</param>
        /// <param name="parentName">Name of the parent, to look for the child</param>
        public UseChildAttribute(string childName, string parentName) : this(childName)
        {
            ParentName = parentName;
        }
    }
}
