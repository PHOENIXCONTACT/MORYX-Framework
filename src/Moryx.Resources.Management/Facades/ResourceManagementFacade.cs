// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Runtime.Modules;

namespace Moryx.Resources.Management
{
    internal class ResourceManagementFacade : FacadeBase, IResourceManagement
    {
        #region Dependency Injection

        public IResourceManager ResourceManager { get; set; }

        public IResourceGraph ResourceGraph { get; set; }

        public IResourceTypeController TypeController { get; set; }

        public IResourceInitializerFactory InitializerFactory { get; set; }

        #endregion

        /// <seealso cref="IFacadeControl"/>
        public override void Activate()
        {
            ResourceManager.ResourceAdded += OnResourceAdded;
            ResourceManager.CapabilitiesChanged += OnCapabilitiesChanged;
            ResourceManager.ResourceRemoved += OnResourceRemoved;
            ResourceManager.ResourceChanged += OnResourceChanged;
        }

        /// <seealso cref="IFacadeControl"/>
        public override void Deactivate()
        {
            ResourceManager.ResourceAdded -= OnResourceAdded;
            ResourceManager.CapabilitiesChanged -= OnCapabilitiesChanged;
            ResourceManager.ResourceRemoved -= OnResourceRemoved;
            ResourceManager.ResourceChanged -= OnResourceChanged;
        }

        private void OnCapabilitiesChanged(object sender, ICapabilities args)
        {
            CapabilitiesChanged?.Invoke(((IResource)sender).Proxify(TypeController), args);
        }

        private void OnResourceAdded(object sender, IResource resource)
        {
            ResourceAdded?.Invoke(this, resource.Proxify(TypeController));
        }

        private void OnResourceChanged(object sender, IResource resource)
        {
            ResourceChanged?.Invoke(this, resource.Proxify(TypeController));
        }

        private void OnResourceRemoved(object sender, IResource resource)
        {
            ResourceRemoved?.Invoke(this, resource.Proxify(TypeController));
        }

        #region IResourceManagement
        public TResource GetResource<TResource>() where TResource : class, IResource
        {
            ValidateHealthState();
            return ResourceGraph.GetResource<TResource>()?.Proxify(TypeController);
        }

        public TResource GetResource<TResource>(long id)
            where TResource : class, IResource
        {
            ValidateHealthState();
            return ResourceGraph.GetResource<TResource>(id)?.Proxify(TypeController);
        }

        public TResource GetResource<TResource>(string name)
            where TResource : class, IResource
        {
            ValidateHealthState();
            return ResourceGraph.GetResource<TResource>(name)?.Proxify(TypeController);
        }

        public TResource GetResource<TResource>(ICapabilities requiredCapabilities)
            where TResource : class, IResource
        {
            ValidateHealthState();
            return ResourceGraph.GetResource<TResource>(r => requiredCapabilities.ProvidedBy(r.Capabilities))?.Proxify(TypeController);
        }

        public TResource GetResource<TResource>(Func<TResource, bool> predicate)
            where TResource : class, IResource
        {
            ValidateHealthState();
            return ResourceGraph.GetResource(predicate)?.Proxify(TypeController);
        }

        public IEnumerable<TResource> GetResources<TResource>() where TResource : class, IResource
        {
            ValidateHealthState();
            return ResourceGraph.GetResources<TResource>().Proxify(TypeController);
        }

        public IEnumerable<TResource> GetResources<TResource>(ICapabilities requiredCapabilities)
            where TResource : class, IResource
        {
            ValidateHealthState();
            return ResourceGraph.GetResources<TResource>(r => requiredCapabilities.ProvidedBy(r.Capabilities)).Proxify(TypeController);
        }

        public IEnumerable<TResource> GetResources<TResource>(Func<TResource, bool> predicate)
            where TResource : class, IResource
        {
            ValidateHealthState();
            return ResourceGraph.GetResources(predicate).Proxify(TypeController);
        }

        #endregion

        public async Task<long> CreateUnsafeAsync(Type resourceType, Func<Resource, Task> initializer, CancellationToken cancellationToken = default)
        {
            ValidateHealthState();

            var resource = ResourceGraph.Instantiate(resourceType.ResourceType());
            await initializer(resource);
            await ResourceGraph.SaveAsync(resource, cancellationToken);
            return resource.Id;
        }


        public TResult ReadUnsafe<TResult>(long id, Func<Resource, TResult> accessor)
        {
            ValidateHealthState();

            var resource = ResourceGraph.Get(id);

            var result = accessor(resource);
            return result;
        }

        public async Task ModifyUnsafeAsync(long id, Func<Resource, Task<bool>> modifier, CancellationToken cancellationToken = default)
        {
            ValidateHealthState();

            var resource = ResourceGraph.Get(id);
            if (resource == null)
                throw new KeyNotFoundException($"No resource with Id {id} found!");

            var result = await modifier(resource);
            if (result)
                await ResourceGraph.SaveAsync(resource, cancellationToken);
        }

        public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            ValidateHealthState();

            var resource = ResourceGraph.Get(id);
            if (resource == null)
                return false;

            return await ResourceGraph.DestroyAsync(resource, cancellationToken);
        }

        public IEnumerable<TResource> GetResourcesUnsafe<TResource>(Func<TResource, bool> predicate)
             where TResource : class, IResource
        {
            ValidateHealthState();
            return ResourceGraph.GetResources(predicate);
        }

        public Task ExecuteInitializerAsync(string initializerName, object parameters, CancellationToken cancellationToken = default)
        {
            ValidateHealthState();

            return ResourceManager.ExecuteInitializer(initializerName, parameters);
        }

        public async Task ExecuteInitializerAsync(ResourceInitializerConfig initializerConfig, object parameters, CancellationToken cancellationToken = default)
        {
            ValidateHealthState();

            var initializer = InitializerFactory.Create(initializerConfig);

            var result = await ResourceManager.ExecuteInitializer(initializer, parameters);

            InitializerFactory.Destroy(initializer);
        }

        public event EventHandler<IResource> ResourceAdded;

        public event EventHandler<IResource> ResourceRemoved;

        public event EventHandler<IResource> ResourceChanged;

        public event EventHandler<ICapabilities> CapabilitiesChanged;
    }

}
