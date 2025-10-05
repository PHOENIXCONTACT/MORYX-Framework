// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.ControlSystem.Materials;

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

        /// <inheritdoc />
        public void Start()
        {
            var containers = ResourceManagement.GetResources<IMaterialContainer>().ToList();
            foreach (var container in containers)
            {
                container.MaterialChanged += OnMaterialChanged;
                SetProduct(container);
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            var containers = ResourceManagement.GetResources<IMaterialContainer>().ToList();
            foreach (var container in containers)
            {
                container.MaterialChanged -= OnMaterialChanged;
            }
        }

        private void OnMaterialChanged(object sender, EventArgs empty)
        {
            var container = (IMaterialContainer)sender;
            SetProduct(container);
        }

        private void SetProduct(IMaterialContainer container)
        {
            if (container.ProvidedMaterial == null)
                return;

            // Update reference with values from product management
            if(container.ProvidedMaterial is ProductReference && container.ProvidedMaterial.Identity is ProductIdentity prodIdent)
            {
                var material = ProductManagement.LoadType(prodIdent);
                container.SetMaterial(material);
            }
        }
    }
}
