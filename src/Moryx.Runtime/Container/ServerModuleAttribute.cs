// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Container
{
    /// <summary>
    /// Attribute to register server modules
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServerModuleAttribute : GlobalComponentAttribute
    {
        /// <summary>
        /// Register this module using its name
        /// </summary>
        /// <param name="name">Name of this module</param>
        public ServerModuleAttribute(string name) : base(LifeCycle.Singleton, typeof(IServerModule))
        {
            Name = name;
        }
    }
}
