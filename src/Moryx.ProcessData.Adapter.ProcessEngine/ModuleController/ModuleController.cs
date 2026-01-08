// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.Processes;
using Moryx.Runtime.Modules;

namespace Moryx.ProcessData.Adapter.ProcessEngine
{
    /// <summary>
    /// Module controller of the process data monitor adapter.
    /// </summary>
    public class ModuleController : ServerModuleBase<ModuleConfig>
    {
        /// <inheritdoc />
        public override string Name => "PdmProcessEngine";

        /// <summary>
        /// ProcessControl facade dependency
        /// </summary>
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public IProcessControl ProcessControl { get; set; }

        /// <summary>
        /// JobManagement facade dependency
        /// </summary>
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public IJobManagement JobManagement { get; set; }

        /// <summary>
        /// Resource management dependency
        /// </summary>
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public IResourceManagement ResourceManagement { get; set; }

        /// <summary>
        /// ProcessDataMonitor facade dependency
        /// </summary>
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public IProcessDataMonitor ProcessDataMonitor { get; set; }

        /// <summary>
        /// Create new instance of process engine adapter
        /// </summary>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
            : base(containerFactory, configManager, loggerFactory)
        {
        }

        /// <inheritdoc />
        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Container.SetInstance(ProcessControl)
                .SetInstance(JobManagement)
                .SetInstance(ProcessDataMonitor)
                .SetInstance(ResourceManagement);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task OnStartAsync(CancellationToken cancellationToken)
        {
            Container.Resolve<ProcessEngineAdapter>().Start();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task OnStopAsync(CancellationToken cancellationToken)
        {
            Container.Resolve<ProcessEngineAdapter>().Stop();
            return Task.CompletedTask;
        }
    }
}
