// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.ProcessData.Adapter.ResourceManagement
{
    /// <summary>
    /// Module controller of the process data monitor adapter.
    /// </summary>
    public class ModuleController : ServerModuleBase<ModuleConfig>
    {
        /// <inheritdoc />
        public override string Name => "PdmResourceManagement";

        /// <summary>
        /// ResourceManagement facade dependency
        /// </summary>
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public IResourceManagement ResourceManagement { get; set; }

        /// <summary>
        /// ProcessDataMonitor facade dependency
        /// </summary>
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public IProcessDataMonitor ProcessDataMonitor { get; set; }

        /// <summary>
        /// Create resource adapter for order data
        /// </summary>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
            : base(containerFactory, configManager, loggerFactory)
        {
        }

        /// <inheritdoc />
        protected override Task OnInitializeAsync()
        {
            Container.SetInstance(ResourceManagement)
                .SetInstance(ProcessDataMonitor);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task OnStartAsync()
        {
            Container.Resolve<ResourceManagementAdapter>().Start();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task OnStopAsync()
        {
            Container.Resolve<ResourceManagementAdapter>().Stop();
            return Task.CompletedTask;
        }
    }
}
