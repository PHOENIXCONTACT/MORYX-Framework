using System;
using System.Linq;
using Marvin.Container;

namespace Marvin.Resources.UI
{
    /// <summary>
    /// Registration attribute for <see cref="IResourceInteractionController"/>
    /// </summary>
    public class ResourceInteractionRegistrationAttribute : PluginAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceInteractionRegistrationAttribute"/> class.
        /// </summary>
        public ResourceInteractionRegistrationAttribute(params Type[] services)
            : base(LifeCycle.Singleton, new[] { typeof(IResourceInteractionController) }.Union(services).ToArray())
        {
        }
    }
}