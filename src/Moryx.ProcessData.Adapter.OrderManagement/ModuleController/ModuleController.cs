// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Orders;
using Moryx.Runtime.Modules;

namespace Moryx.ProcessData.Adapter.OrderManagement
{
    /// <summary>
    /// Module controller of the process data monitor adapter.
    /// </summary>
    public class ModuleController : ServerModuleBase<ModuleConfig>
    {
        /// <inheritdoc />
        public override string Name => "PdmOrderManagement";

        /// <summary>
        /// ProcessControl facade dependency
        /// </summary>
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public IOrderManagement OrderManagement { get; set; }

        /// <summary>
        /// ProcessDataMonitor facade dependency
        /// </summary>
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public IProcessDataMonitor ProcessDataMonitor { get; set; }

        /// <summary>
        /// Create order adapter for process data
        /// </summary>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
            : base(containerFactory, configManager, loggerFactory)
        {
        }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            Container.SetInstance(OrderManagement)
                .SetInstance(ProcessDataMonitor);
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            Container.Resolve<OrderManagementAdapter>().Start();
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            Container.Resolve<OrderManagementAdapter>().Stop();
        }
    }
}
