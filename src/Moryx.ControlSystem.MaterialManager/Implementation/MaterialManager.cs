// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.ControlSystem.Materials;
using Moryx.Logging;

namespace Moryx.ControlSystem.MaterialManager
{
    [Component(LifeCycle.Singleton, typeof(IMaterialManager))]
    internal class MaterialManager : IMaterialManager
    {
        /// <summary>
        /// Access material containers
        /// </summary>
        public IResourceManagement ResourceManagement { get; set; }

        /// <summary>
        /// Load the products of each container
        /// </summary>
        public IProductManagement ProductManagement { get; set; }

        /// <summary>
        /// Logger for this component
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <inheritdoc />
        public async Task StartAsync()
        {
            var containers = ResourceManagement.GetResources<IMaterialContainer>().ToList();
            foreach (var container in containers)
            {
                container.MaterialChanged += OnMaterialChanged;
                await SetProduct(container);
            }
        }

        /// <inheritdoc />
        public Task StopAsync()
        {
            var containers = ResourceManagement.GetResources<IMaterialContainer>().ToList();
            foreach (var container in containers)
            {
                container.MaterialChanged -= OnMaterialChanged;
            }

            return Task.CompletedTask;
        }

        private async void OnMaterialChanged(object sender, EventArgs empty)
        {
            try
            {
                var container = (IMaterialContainer)sender;
                await SetProduct(container);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during setting product");
            }
        }

        private async Task SetProduct(IMaterialContainer container)
        {
            if (container.ProvidedMaterial == null)
                return;

            // Update reference with values from product management
            if (container.ProvidedMaterial is ProductReference && container.ProvidedMaterial.Identity is ProductIdentity prodIdent)
            {
                var material = await ProductManagement.LoadTypeAsync(prodIdent);
                container.SetMaterial(material);
            }
        }
    }
}
