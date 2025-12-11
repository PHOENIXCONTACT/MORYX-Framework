// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.ControlSystem.MaterialManager
{
    /// <summary>
    /// Module controller of the MaterialManager.
    /// </summary>
    [Description("Module responsible for material containers and material planning")]
    public class ModuleController : ServerModuleBase<ModuleConfig>
    {
        /// <summary>
        /// The module's name.
        /// </summary>
        public const string ModuleName = "MaterialManager";

        /// <summary>
        /// Create new module instance
        /// </summary>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory) : base(containerFactory, configManager, loggerFactory)
        {
        }

        /// <inheritdoc />
        public override string Name => ModuleName;

        #region Generated imports

        /// <summary>
        /// Resource management facade that allows communication with different hardware within
        /// the machine
        /// </summary>
        [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
        public IResourceManagement ResourceManagement { get; set; }

        /// <summary>
        /// Product management to load and save articles instances and products
        /// </summary>
        [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
        public IProductManagement ProductManagement { get; set; }

        #endregion

        #region State transition

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override Task OnInitializeAsync()
        {
            Container
                .SetInstance(ResourceManagement)
                .SetInstance(ProductManagement);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override Task OnStartAsync()
        {
            // Start material manager
            return Container.Resolve<IMaterialManager>().StartAsync();
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override Task OnStopAsync()
        {
            return Container.Resolve<IMaterialManager>().StopAsync();
        }

        #endregion
    }
}
