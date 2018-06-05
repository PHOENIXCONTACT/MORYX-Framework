using System;

namespace Marvin.Container
{
    /// <summary>
    /// Base attribute for all registrations of the global container
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GlobalComponentAttribute : RegistrationAttribute
    {
        /// <summary>
        /// Constructor with life cycle
        /// </summary>
        /// <param name="lifeStyle">Life style of component</param><param name="services">Implemented service</param>
        public GlobalComponentAttribute(LifeCycle lifeStyle, params Type[] services) : base(lifeStyle, services)
        {
        }
    }
}
