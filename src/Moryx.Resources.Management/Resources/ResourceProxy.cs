// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management
{
    /// <summary>
    /// Base type for all proxies
    /// </summary>
    internal abstract class ResourceProxy : IResource
    {
        /// <summary>
        /// Type controller field to convert references to public resources before returning them
        /// </summary>
        private IResourceTypeController _typeController;

        public event EventHandler<ICapabilities> CapabilitiesChanged
        {
            add { Target.CapabilitiesChanged += value; }
            remove { Target.CapabilitiesChanged -= value; }
        }

        /// <summary>
        /// Target resource of the proxy
        /// </summary>
        public IResource Target { get; private set; }

        /// <summary>
        /// Create proxy for a given target
        /// </summary>
        protected ResourceProxy(IResource target, IResourceTypeController typeController)
        {
            Target = target;
            _typeController = typeController;
        }

        /// <inheritdoc />
        long IResource.Id => Target.Id;

        /// <inheritdoc />
        string IResource.Name => Target.Name;

        public ICapabilities Capabilities => Target.Capabilities;

        public virtual void Attach()
        {
        }

        public virtual void Detach()
        {
            _typeController = null;
            Target = null;
        }

        public override string ToString()
        {
            return Target.ToString();
        }

        /// <summary>
        /// Convert a referenced instance to a proxy
        /// </summary>
        protected internal TResource Convert<TResource>(IResource instance)
            where TResource : IResource
        {
            return (TResource)_typeController.GetProxy((Resource)instance);
        }

        /// <summary>
        /// Convert a collection of referenced resources to proxies
        /// </summary>
        protected internal TResource[] ConvertMany<TResource>(IEnumerable<IResource> instances)
            where TResource : IResource
        {
            return instances.Select(Convert<TResource>).ToArray();
        }

        /// <summary>
        /// Extract the target object from a proxy
        /// </summary>
        protected internal static TResource Extract<TResource>(IResource instance)
            where TResource : IResource
        {
            var proxy = (ResourceProxy)instance;
            return (TResource)proxy.Target;
        }

        /// <summary>
        /// Extract target objects from collection of proxies
        /// </summary>
        protected internal static TResource[] ExtractMany<TResource>(IEnumerable<IResource> instances)
            where TResource : IResource
        {
            return instances.Select(Extract<TResource>).ToArray();
        }
    }

    /// <summary>
    /// Resource proxy base for typed access to resources
    /// </summary>
    internal abstract class ResourceProxy<TTarget> : ResourceProxy
        where TTarget : Resource
    {
        /// <summary>
        /// Typed access to the target field
        /// </summary>
        public new TTarget Target
        {
            get
            {
                if (base.Target == null)
                    throw new ProxyDetachedException();

                return (TTarget)base.Target;
            }
        }

        /// <summary>
        /// Create proxy for a given target
        /// </summary>
        protected ResourceProxy(TTarget target, IResourceTypeController typeController) : base(target, typeController)
        {
        }
    }
}
