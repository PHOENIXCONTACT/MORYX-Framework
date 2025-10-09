// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Modules;

namespace Moryx.Resources.Management
{
    internal class ResourceManagementFacade : ResourceManagement
    {
        #region Dependency Injection

        public IResourceManager Manager { get; set; }

        public IResourceGraph ResourceGraph { get; set; }

        public IResourceTypeController TypeController { get; set; }

        #endregion

        #region IFacadeControl

        /// <seealso cref="IFacadeControl"/>
        public override void Activated()
        {
            Manager.ResourceAdded += OnResourceAdded;
            Manager.CapabilitiesChanged += OnCapabilitiesChanged;
            Manager.ResourceRemoved += OnResourceRemoved;
        }

        /// <seealso cref="IFacadeControl"/>
        public override void Deactivate()
        {
            Manager.ResourceAdded -= OnResourceAdded;
            Manager.CapabilitiesChanged -= OnCapabilitiesChanged;
            Manager.ResourceRemoved -= OnResourceRemoved;
        }

        #endregion

        private void OnCapabilitiesChanged(object sender, ICapabilities args)
        {
            RaiseCapabilitiesChanged(((IResource)sender).Proxify(TypeController), args);
        }

        private void OnResourceAdded(object sender, IResource publicResource)
        {
            RaiseResourceAdded(this, publicResource.Proxify(TypeController));
        }

        private void OnResourceRemoved(object sender, IResource publicResource)
        {
            RaiseResourceRemoved(this, publicResource.Proxify(TypeController));
        }

        public override TResource GetResource<TResource>()
        {
            ValidateHealthState();
            return ResourceGraph.GetResource<TResource>()?.Proxify(TypeController);
        }

        public override TResource GetResource<TResource>(long id)
        {
            ValidateHealthState();
            return ResourceGraph.GetResource<TResource>(id)?.Proxify(TypeController);
        }

        public override TResource GetResource<TResource>(string name)
        {
            ValidateHealthState();
            return ResourceGraph.GetResource<TResource>(name)?.Proxify(TypeController);
        }

        public override TResource GetResource<TResource>(ICapabilities requiredCapabilities)
        {
            ValidateHealthState();
            return ResourceGraph.GetResource<TResource>(r => requiredCapabilities.ProvidedBy(r.Capabilities))?.Proxify(TypeController);
        }

        public override TResource GetResource<TResource>(Func<TResource, bool> predicate)
        {
            ValidateHealthState();
            return ResourceGraph.GetResource(predicate)?.Proxify(TypeController);
        }

        public override IEnumerable<TResource> GetResources<TResource>()
        {
            ValidateHealthState();
            return ResourceGraph.GetResources<TResource>().Proxify(TypeController);
        }

        public override IEnumerable<TResource> GetResources<TResource>(ICapabilities requiredCapabilities)
        {
            ValidateHealthState();
            return ResourceGraph.GetResources<TResource>(r => requiredCapabilities.ProvidedBy(r.Capabilities)).Proxify(TypeController);
        }

        public override IEnumerable<TResource> GetResources<TResource>(Func<TResource, bool> predicate)
        {
            ValidateHealthState();
            return ResourceGraph.GetResources(predicate).Proxify(TypeController);
        }

        public override long Create(Type resourceType, Action<Resource> initializer)
        {
            ValidateHealthState();

            var resource = ResourceGraph.Instantiate(resourceType.ResourceType());
            initializer(resource);
            ResourceGraph.Save(resource);
            return resource.Id;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="id"></param>
        /// <param name="accessor"></param>
        /// <returns>null, when a resource with this id doesn't exist</returns>
        public override TResult Read<TResult>(long id, Func<Resource, TResult> accessor)
        {
            ValidateHealthState();

            var resource = ResourceGraph.Get(id);

            var result = accessor(resource);
            return result;
        }

        public override void Modify(long id, Func<Resource, bool> modifier)
        {
            ValidateHealthState();

            var resource = ResourceGraph.Get(id);
            if (resource == null)
                throw new KeyNotFoundException($"No resource with Id {id} found!");

            var result = modifier(resource);
            if (result)
                ResourceGraph.Save(resource);
        }

        public override bool Delete(long id)
        {
            ValidateHealthState();

            var resource = ResourceGraph.Get(id);
            if (resource == null)
                return false;

            return ResourceGraph.Destroy(resource);
        }

        public override IEnumerable<TResource> GetAllResources<TResource>(Func<TResource, bool> predicate)
        {
            ValidateHealthState();
            return ResourceGraph.GetResources(predicate);
        }
    }
}
