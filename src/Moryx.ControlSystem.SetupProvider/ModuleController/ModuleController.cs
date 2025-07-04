// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.ControlSystem.Setups;
using Moryx.Runtime.Modules;

#if COMMERCIAL
using System;
#endif

namespace Moryx.ControlSystem.SetupProvider
{
    /// <summary>
    /// Module controller of the SetupProvider
    /// </summary>
    [Description("SetupProvider to determine setup requirement and generate reconfiguration workplans")]
    public class ModuleController : ServerModuleBase<ModuleConfig>, IFacadeContainer<ISetupProvider>
    {
        /// <summary>
        /// The module's name.
        /// </summary>
        public const string ModuleName = "SetupProvider";

        /// <inheritdoc />
        public override string Name => ModuleName;

        /// <summary>
        /// Create new module instance
        /// </summary>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory) : base(containerFactory, configManager, loggerFactory)
        {
        }

        #region State transition

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override void OnInitialize()
        {
            // Register plugins for the setup management
            Container.LoadComponents<ISetupTrigger>();

#if COMMERCIAL
            if (LicenseCheck.IsDeveloperLicense())
                Logger.Log(LogLevel.Warning, "Running with developer license for 1h");
#endif
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override void OnStart()
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                throw new InvalidOperationException("No license available!");
#endif

            // Resolve component orchestration and start all components in the correct order
            Container.Resolve<ISetupManager>().Start();

            // Activate facade
            ActivateFacade(_setupProviderFacade);
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override void OnStop()
        {
            // Deactivate facade
            DeactivateFacade(_setupProviderFacade);

            Container.Resolve<ISetupManager>().Stop();
        }

        #endregion

        #region FacadeContainer

        private readonly SetupProviderFacade _setupProviderFacade = new();

        ISetupProvider IFacadeContainer<ISetupProvider>.Facade => _setupProviderFacade;

        #endregion
    }
}
