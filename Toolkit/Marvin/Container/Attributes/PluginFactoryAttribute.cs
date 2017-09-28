using System;
using Marvin.Testing;

namespace Marvin.Container
{
    /// <summary>
    /// Interface for plguin factories within the local container
    /// </summary>
    [OpenCoverIgnore]
    [AttributeUsage(AttributeTargets.Interface)]
    public class PluginFactoryAttribute : FactoryRegistrationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public PluginFactoryAttribute()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectorType"></param>
        public PluginFactoryAttribute(Type selectorType) : base(selectorType)
        {

        }
    }
}
