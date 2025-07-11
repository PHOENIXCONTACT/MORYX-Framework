// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.ProcessData.Listener;
using Moryx.Runtime.Modules;

#if COMMERCIAL
using System;
#endif

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
#if COMMERCIAL
            if (LicenseCheck.IsDeveloperLicense())
                Logger.Log(LogLevel.Warning, "Running with developer license for 1h");
#endif
            Container.SetInstance(ConfigManager);

            Container.LoadComponents<IProcessDataListener>();
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                throw new InvalidOperationException("No license available!");
#endif
            Container.Resolve<IProcessDataCollector>().Start();
            ActivateFacade(_processDataMonitorFacade);
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            Container.Resolve<IProcessDataCollector>().Stop();
            DeactivateFacade(_processDataMonitorFacade);
        }

        private readonly ProcessDataMonitorFacade _processDataMonitorFacade = new ProcessDataMonitorFacade();
        IProcessDataMonitor IFacadeContainer<IProcessDataMonitor>.Facade => _processDataMonitorFacade;
    }
}
