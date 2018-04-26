using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer.Capabilities;
using Marvin.AbstractionLayer.Resources;
using Marvin.Runtime.Modules;

namespace Marvin.Resources.Management
{
    internal class ResourceManagementFacade : IResourceManagement, IFacadeControl
    {
        #region Dependency Injection

        public IResourceManager Manager { get; set; }

        public IResourceTypeController TypeController { get; set; }

        #endregion

        /// <see cref="IFacadeControl.ValidateHealthState"/> 
        public Action ValidateHealthState { get; set; }

        /// <seealso cref="IFacadeControl"/> 
        public void Activate()
        {
            Manager.ResourceAdded += OnResourceAdded;
            Manager.CapabilitiesChanged += OnCapabilitiesChanged;
        }
        
        /// <seealso cref="IFacadeControl"/> 
        public void Deactivate()
        {
            Manager.ResourceAdded -= OnResourceAdded;
            Manager.CapabilitiesChanged -= OnCapabilitiesChanged;
        }


        private void OnCapabilitiesChanged(object sender, ICapabilities args)
        {
            CapabilitiesChanged?.Invoke(((IPublicResource)sender).Proxify(TypeController), args);
        }

        private void OnResourceAdded(object sender, IPublicResource publicResource)
        {
            ResourceAdded?.Invoke(this, publicResource.Proxify(TypeController));
        }

        public TResource GetResource<TResource>() where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResource<TResource>().Proxify(TypeController);
        }

        public TResource GetResource<TResource>(long id) 
            where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResource<TResource>(id).Proxify(TypeController);
        }

        public TResource GetResource<TResource>(string name) 
            where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResource<TResource>(name).Proxify(TypeController);
        }

        public TResource GetResource<TResource>(ICapabilities requiredCapabilities) 
            where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResource<TResource>(requiredCapabilities).Proxify(TypeController);
        }

        public TResource GetResource<TResource>(Func<TResource, bool> predicate) 
            where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResource(predicate).Proxify(TypeController);
        }

        public IEnumerable<TResource> GetResources<TResource>() where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResources<TResource>().Proxify(TypeController);
        }

        public IEnumerable<TResource> GetResources<TResource>(ICapabilities requiredCapabilities)
            where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResources<TResource>(requiredCapabilities).Proxify(TypeController);
        }
        
        public IEnumerable<TResource> GetResources<TResource>(Func<TResource, bool> predicate) 
            where TResource : class, IPublicResource
        {
            ValidateHealthState();
            return Manager.GetResources(predicate).Proxify(TypeController);
        }

        /// <inheritdoc />
        public event EventHandler<IPublicResource> ResourceAdded;

        /// <inheritdoc />
        public event EventHandler<ICapabilities> CapabilitiesChanged;
    }
}