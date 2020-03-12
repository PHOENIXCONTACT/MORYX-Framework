// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Marvin.Container;

namespace Marvin.Runtime.Modules
{
    /// <summary>
    /// Attribute for implementations of <see cref="IServerModuleConsole"/>
    /// to register in container structure
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServerModuleConsoleAttribute : PluginAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerModuleConsoleAttribute"/> class.
        /// </summary>
        public ServerModuleConsoleAttribute()
            : base(LifeCycle.Singleton, typeof(IServerModuleConsole))
        {
        }
    }
}
