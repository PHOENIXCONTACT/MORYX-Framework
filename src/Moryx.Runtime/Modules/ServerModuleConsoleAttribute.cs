// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Runtime.Modules
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
