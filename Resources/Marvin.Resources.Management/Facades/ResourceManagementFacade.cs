using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Capabilities;
using Marvin.AbstractionLayer.Resources;
using Marvin.Runtime.Base;

namespace Marvin.Resources.Management
{
    internal class ResourceManagementFacade : IResourceManagement, IFacadeControl
    {
        #region Dependency Injection

        public IResourceManager Manager { get; set; }

        #endregion

        /// <see cref="IFacadeControl.ValidateHealthState"/> 
        public Action ValidateHealthState { get; set; }

        /// <seealso cref="IFacadeControl"/> 
        public void Activate()
        {
            Manager.ResourceAdded += OnResourceAdded;
            Manager.CapabilitiesChanged += ManagerOnCapabilitiesChanged;
        }

        
        /// <seealso cref="IFacadeControl"/> 
        public void Deactivate()
        {
            Manager.ResourceAdded -= OnResourceAdded;
            Manager.CapabilitiesChanged -= ManagerOnCapabilitiesChanged;
        }


        private void ManagerOnCapabilitiesChanged(object sender, ICapabilities args)
        {
            CapabilitiesChanged?.Invoke(sender, args);
        }

        private void OnResourceAdded(object sender, IPublicResource publicResource)
        {
            ResourceAdded?.Invoke(this, publicResource);
        }

        public TResource GetResource<TResource>() where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResource<TResource>();
        }


        public TResource GetResource<TResource>(long id) where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResource<TResource>(id);
        }

        public TResource GetResource<TResource>(string name) where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResource<TResource>(name);
        }

        public TResource GetResource<TResource>(ICapabilities requiredCapabilities) where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResource<TResource>(requiredCapabilities);
        }

        public TResource GetResource<TResource>(Func<TResource, bool> predicate) where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResource(predicate);
        }

        public IEnumerable<TResource> GetResources<TResource>() where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResources<TResource>();
        }

        public IEnumerable<TResource> GetResources<TResource>(ICapabilities requiredCapabilities) where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResources<TResource>(requiredCapabilities);
        }


        public IEnumerable<TResource> GetResources<TResource>(Func<TResource, bool> predicate) where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResources(predicate);
        }

        /// <inheritdoc />
        public event EventHandler<IPublicResource> ResourceAdded;

        /// <summary>
        /// <seealso cref="IResourceManagement"/>
        /// </summary>
        public event EventHandler<ICapabilities> CapabilitiesChanged;
    }
}