// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Members of the given interfaces are available outside of the resource management
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ResourceAvailableAsAttribute : Attribute
    {
        /// <summary>
        /// Public available type definitions
        /// </summary>
        public Type[] AvailableAs { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ResourceAvailableAsAttribute"/>
        /// </summary>
        public ResourceAvailableAsAttribute(params Type[] interfaces)
        {
            AvailableAs = interfaces;
        }
    }
}
