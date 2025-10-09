// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Modules;
using Moryx.ProcessData.Listener;
using Moryx.Runtime.Modules;

namespace Moryx.ProcessData.Monitor
{
    /// <summary>
    /// Module controller of the process data monitor.
    /// </summary>
    public class ModuleController : ServerModuleBase<ModuleConfig>, IFacadeContainer<IProcessDataMonitor>
    {
        /// <inheritdoc />
        public override string Name => "ProcessDataMonitor";

        /// <summary>
        /// Create instance of process data monitor
        /// </summary>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
            : base(containerFactory, configManager, loggerFactory)
        {
        }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            Container.SetInstance(ConfigManager);

            Container.LoadComponents<IProcessDataListener>();
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            Container.Resolve<IProcessDataCollector>().Start();
            ActivateFacade(_processDataMonitorFacade);
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            Container.Resolve<IProcessDataCollector>().Stop();
            DeactivateFacade(_processDataMonitorFacade);
        }

        private readonly ProcessDataMonitorFacade _processDataMonitorFacade = new();
        IProcessDataMonitor IFacadeContainer<IProcessDataMonitor>.Facade => _processDataMonitorFacade;
    }
}
