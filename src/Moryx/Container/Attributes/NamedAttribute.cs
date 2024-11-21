// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Container
{
    /// <summary>
    /// Attribute for properties in Castle using named components
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class NamedAttribute : Attribute
    {
        /// <summary>
        /// Create a new instance of the <see cref="NamedAttribute"/>
        /// </summary>
        /// <param name="name">Name of the component that shall be injected</param>
        public NamedAttribute(string name)
        {
            ComponentName = name;
        }

        /// <summary>
        /// Name of component to inject at this point
        /// </summary>
        public string ComponentName { get; set; }
    }
}
