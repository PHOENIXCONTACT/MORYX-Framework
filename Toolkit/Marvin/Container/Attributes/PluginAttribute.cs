using System;
using Marvin.Testing;

namespace Marvin.Container
{
    /// <summary>
    /// Registration attribute for local container plugins
    /// </summary>
    [OpenCoverIgnore]
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
