using System;
using Marvin.Testing;

namespace Marvin.Container
{
    /// <summary>
    /// Registration attribute to decorate components of a module.
    /// </summary>
    [OpenCoverIgnore]
    public class ComponentAttribute : RegistrationAttribute
    {
        /// <summary>
        /// Constructor with life cycle
        /// </summary>
        /// <param name="lifeStyle">Life style of component</param>
        /// <param name="services">Implemented service</param>
        public ComponentAttribute(LifeCycle lifeStyle, params Type[] services) 
            : base(lifeStyle, services)
        {
        }

        /// <summary>
        /// Flag that this plugin shall not be intercepted
        /// </summary>
        public bool DontIntercept { get; set; }    
    }
}