﻿using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.ControlSystem.VisualInstructions;
using Moryx.Runtime.Modules;

#if COMMERCIAL
using Moryx.Logging;
using System;
#endif

namespace Moryx.ControlSystem.WorkerSupport
{
    /// <summary>
    /// Module controller of the ProcessEngine
    /// </summary>
    [Description("Server module to manage and publish worker support instructions")]
    public class ModuleController : ServerModuleBase<ModuleConfig>, IFacadeContainer<IWorkerSupport>
    {
        /// <summary>
        /// The module's name.
        /// </summary>
        public const string ModuleName = "WorkerSupport";

        /// <inheritdoc />
        public override string Name => ModuleName;

        /// <summary>
        /// Create new module instance
        /// </summary>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory) : base(containerFactory, configManager, loggerFactory)
        {
        }

        #region Generated imports

        /// <summary>
        /// Resource management facade that allows communication with different hardware within
        /// the machine
        /// </summary>
        [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
        public IResourceManagement ResourceManagement { get; set; }

        #endregion

        #region State transition

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override void OnInitialize()
        {
            // Register all imported components
            Container.SetInstance(ResourceManagement);
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
            Container.Resolve<IWorkerSupportController>().Start();

            // Activate facade
            ActivateFacade(_workerSupportFacade);
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override void OnStop()
        {
            // Deactivate facade
            DeactivateFacade(_workerSupportFacade);


            Container.Resolve<IWorkerSupportController>().Stop();
        }

        #endregion

        #region FacadeContainer

        private readonly WorkerSupportFacade _workerSupportFacade = new();

        IWorkerSupport IFacadeContainer<IWorkerSupport>.Facade => _workerSupportFacade;

        #endregion
    }
}