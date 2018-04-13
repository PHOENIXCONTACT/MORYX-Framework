using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer.Capabilities;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Hidden interface for <see cref="ResourceProxy{TTarget}"/> to clear
    /// the reference to the original object
    /// </summary>
    internal interface IResourceProxy : IResource
    {

        /// <summary>
        /// Target object of the proxy
        /// </summary>
        IResource Target { get; }

        /// <summary>
        /// Attach to the target object and its events
        /// </summary>
        void Attach();

        /// <summary>
        /// Detach proxy from target object
        /// </summary>
        void Detach();
    }

    /// <summary>
    /// Resource proxy base for typed access to resources
    /// </summary>
    internal abstract class ResourceProxy<TTarget> : IResourceProxy
        where TTarget : Resource
    {
        /// <summary>
        /// Type controller field to convert references to public resources before returning them
        /// </summary>
        private IResourceTypeController _typeController;

        private TTarget _target;
        /// <summary>
        /// Target resource of this proxy
        /// </summary>
        protected internal TTarget Target
        {
            get
            {
                if(_target == null)
                    throw new ProxyDetachedException();

                return _target;
            }
            private set { _target = value; }
        }


        /// <inheritdoc />
        IResource IResourceProxy.Target => Target;

        /// <summary>
        /// Create proxy for a given target
        /// </summary>
        protected ResourceProxy(TTarget target, IResourceTypeController typeController)
        {
            Target = target;
            _typeController = typeController;
        }

        /// <inheritdoc />
        long IResource.Id => Target.Id;

        /// <inheritdoc />
        string IResource.Name => Target.Name;

        /// <inheritdoc />
        string IResource.LocalIdentifier => Target.LocalIdentifier;

        /// <inheritdoc />
        string IResource.GlobalIdentifier => Target.GlobalIdentifier;

        public virtual void Attach()
        {
        }

        public virtual void Detach()
        {
            _typeController = null;
            Target = null;
        }

        /// <summary>
        /// Convert a referenced instance to a proxy
        /// </summary>
        protected internal TResource Convert<TResource>(IResource instance)
            where TResource : IResource
        {
            return (TResource) _typeController.GetProxy((Resource) instance);
        }

        /// <summary>
        /// Convert a collection of referenced resources to proxies
        /// </summary>
        protected internal TResource[] ConvertMany<TResource>(IEnumerable<IResource> instances) 
            where TResource : IResource
        {
            return instances.Select(Convert<TResource>).ToArray();
        }

        public override string ToString()
        {
            return Target.ToString();
        }
    }
}