// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Container
{
    /// <summary>
    /// Registration attribute for local container plugins
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute : ComponentAttribute
    {
        /// <summary>
        /// Constructor with life cycle
        /// </summary>
        /// <param name="lifeStyle">Life style of component</param>
        /// <param name="services">Implemented service</param>
        public PluginAttribute(LifeCycle lifeStyle, params Type[] services) 
            : base(lifeStyle, services)
        {
        }
    }
}
