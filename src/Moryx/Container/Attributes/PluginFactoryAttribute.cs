// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container
{
    /// <summary>
    /// Interface for plguin factories within the local container
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class PluginFactoryAttribute : Attribute
    {
        /// <summary>
        /// Optional component selector
        /// </summary>
        public Type Selector
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PluginFactoryAttribute()
        {
        }

        /// <summary>
        /// Constructor with component selector
        /// </summary>
        /// <param name="selectorType">Selector</param>
        public PluginFactoryAttribute(Type selectorType)
        {
            Selector = selectorType;
        }
    }
}
