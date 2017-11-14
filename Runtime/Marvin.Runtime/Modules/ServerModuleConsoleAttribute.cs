using System;
using Marvin.Container;

namespace Marvin.Runtime.Modules
{
    /// <summary>
    /// Attribute for implementations of <see cref="IServerModuleConsole"/>
    /// to register in container structure
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
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