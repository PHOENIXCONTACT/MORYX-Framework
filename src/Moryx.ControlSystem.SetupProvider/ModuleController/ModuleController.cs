// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.ControlSystem.Setups;
using Moryx.Runtime.Modules;

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
        protected override Task OnInitializeAsync()
        {
            // Register plugins for the setup management
            Container.LoadComponents<ISetupTrigger>();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override Task OnStartAsync()
        {

            // Resolve component orchestration and start all components in the correct order
            Container.Resolve<ISetupManager>().Start();

            // Activate facade
            ActivateFacade(_setupProviderFacade);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override Task OnStopAsync()
        {
            // Deactivate facade
            DeactivateFacade(_setupProviderFacade);

            Container.Resolve<ISetupManager>().Stop();
            return Task.CompletedTask;
        }

        #endregion

        #region FacadeContainer

        private readonly SetupProviderFacade _setupProviderFacade = new();

        ISetupProvider IFacadeContainer<ISetupProvider>.Facade => _setupProviderFacade;

        #endregion
    }
}
