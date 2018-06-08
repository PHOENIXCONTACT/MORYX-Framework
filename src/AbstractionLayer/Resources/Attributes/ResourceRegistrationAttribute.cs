using System;
using Marvin.Container;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Simplified plugin registration attribute for resources
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ResourceRegistrationAttribute : PluginAttribute
    {
        /// <summary>
        /// Generic registration with lifecylce <see cref="LifeCycle.Transient"/>
        /// </summary>
        public ResourceRegistrationAttribute(string name) 
            : base(LifeCycle.Transient, typeof(IResource))
        {
            Name = name;
        }

        /// <summary>
        /// Constructor of custom type with lifecycle <see cref="LifeCycle.Singleton"/>
        /// </summary>
        public ResourceRegistrationAttribute(string name, Type customRegistration)
            : base(LifeCycle.Singleton, typeof(IResource), customRegistration)
        {
            Name = name;
        }
    }
}