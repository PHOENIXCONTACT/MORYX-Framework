using System;
using Marvin.Container;

namespace Marvin.AbstractionLayer.UI
{
    /// <summary>
    /// Base registration attribute for detail view models
    /// </summary>
    public abstract class DetailsRegistrationAttribute : PluginAttribute
    {
        /// <summary>
        /// Type of the view model
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailsRegistrationAttribute"/> class.
        /// </summary>
        protected DetailsRegistrationAttribute(string typeName, Type detailsType) 
            : base(LifeCycle.Transient, detailsType)
        {
            TypeName = typeName;
        }
    }
}