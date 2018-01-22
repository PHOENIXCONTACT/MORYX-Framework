using System;
using Marvin.AbstractionLayer.UI;

namespace Marvin.Resources.UI
{
    /// <summary>
    /// Registration attribute for the resource detauls view models
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ResourceDetailsRegistrationAttribute : DetailsRegistrationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceDetailsRegistrationAttribute"/> class.
        /// </summary>
        /// <param name="typeName">Name of the resource type.</param>
        public ResourceDetailsRegistrationAttribute(string typeName) 
            : base(typeName, typeof(IResourceDetails))
        {
        }
    }
}