using System;
using Marvin.Testing;

namespace Marvin.Container
{
    /// <summary>
    /// Installation attribute for castle windsor
    /// </summary>
    [OpenCoverIgnore]
    [AttributeUsage(AttributeTargets.Class)]
    public class RegistrationAttribute : Attribute
    {
        /// <summary>
        /// Life cycle of this component
        /// </summary>
        public LifeCycle LifeStyle { get; private set; }

        /// <summary>
        /// Implemented service
        /// </summary>
        public Type[] Services { get; private set; }

        /// <summary>
        /// Optional name of component
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor with life cycle
        /// </summary>
        /// <param name="lifeStyle">Life style of component</param>
        /// <param name="services">Implemented service</param>
        public RegistrationAttribute(LifeCycle lifeStyle, params Type[] services)
        {
            LifeStyle = lifeStyle;
            Services = services;
        }
    }

    /// <summary>
    /// Lifecylce for this component
    /// </summary>
    public enum LifeCycle
    {
        /// <summary>
        /// Create only one instance during container life time
        /// </summary>
        Singleton,
        /// <summary>
        /// Create a new instance for every request
        /// </summary>
        Transient,
        /// <summary>
        /// Create a non tracked instance for every request
        /// </summary>
        NoTracking
    }
}
