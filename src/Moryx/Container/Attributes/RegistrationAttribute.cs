// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Container
{
    /// <summary>
    /// Installation attribute for castle windsor
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegistrationAttribute : Attribute
    {
        /// <summary>
        /// Life cycle of this component
        /// </summary>
        public LifeCycle LifeStyle { get; }

        /// <summary>
        /// Implemented service
        /// </summary>
        public Type[] Services { get; }

        /// <summary>
        /// Optional name of component
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor with life cycle
        /// </summary>
        /// <param name="lifeStyle">Life style of component</param>
        /// <param name="services">Implemented service</param>
        public RegistrationAttribute(LifeCycle lifeStyle, params Type[] services)
        {
            LifeStyle = lifeStyle;
            Services = services;
        }
    }

    /// <summary>
    /// Lifecylce for this component
    /// </summary>
    public enum LifeCycle
    {
        /// <summary>
        /// Create only one instance during container life time
        /// </summary>
        Singleton,

        /// <summary>
        /// Create a new instance for every request
        /// </summary>
        Transient
    }
}
