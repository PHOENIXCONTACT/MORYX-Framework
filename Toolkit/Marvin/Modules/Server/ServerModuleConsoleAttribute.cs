using System;
using Marvin.Container;
using Marvin.Testing;

namespace Marvin.Modules.Server
{
    /// <summary>
    /// Attribute for implementations of <see cref="IServerModuleConsole"/>
    /// to register in container structure
    /// </summary>
    [OpenCoverIgnore]
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