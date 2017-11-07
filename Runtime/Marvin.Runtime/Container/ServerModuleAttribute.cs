using System;
using Marvin.Container;

namespace Marvin.Runtime.Container
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
