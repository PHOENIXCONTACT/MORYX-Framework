using System;
using Marvin.AbstractionLayer.Capabilities;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Base class for all public resources
    /// </summary>
    public abstract class PublicResource : Resource, IPublicResource
    {
        /// <summary>
        /// Current capabilities of this resource
        /// </summary>
        private ICapabilities _capabilities = NullCapabilities.Instance;
        /// 
        public ICapabilities Capabilities
        {
            get
            {
                return _capabilities;
            }
            protected set
            {
                _capabilities = value;
                // ReSharper disable once PossibleNullReferenceException <-- Event must always be wired by resource manager
                CapabilitiesChanged(this, _capabilities);
            }
        }

        /// <summary>
        /// <seealso cref="IResource"/>
        /// </summary>
        public event EventHandler<ICapabilities> CapabilitiesChanged;
    }
}