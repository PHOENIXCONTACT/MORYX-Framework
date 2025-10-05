// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.Processes;
using Moryx.ControlSystem.Setups;
using Moryx.Model;
using Moryx.Notifications;
using Moryx.Runtime.Modules;


namespace Moryx.ControlSystem.ProcessEngine
{
    /// <summary>
    /// Module controller of the ProcessEngine
    /// </summary>
    [Description("Central server module to manage, schedule and execute jobs within the Moryx control system. " +
                 "Job execution is influenced by a jobs constraint and evaluated by dedicated components of the DefaultJobScheduler. " +
                 "Jobs may include but are not limited to production, maintenance and resource configuration.")]
    public class ModuleController : ServerModuleBase<ModuleConfig>,
        IFacadeContainer<IJobManagement>, IFacadeContainer<INotificationSource>, IFacadeContainer<IProcessControl>
    {
        /// <summary>
        /// The module's name.
        /// </summary>
        public const string ModuleName = "ProcessEngine";

        /// <inheritdoc />
        public override string Name => ModuleName;

        /// <summary>
        /// Create a new instance of the module
        /// </summary>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager dbContextManager) : base(containerFactory, configManager, loggerFactory)
        {
            DbContextManager = dbContextManager;
        }

#region Generated imports

        /// <summary>
        /// Provides access to the ControlSystem model that stores jobs, processes and activities
        /// </summary>
        public IDbContextManager DbContextManager { get; }

        /// <summary>
        /// Resource management facade that allows communication with different hardware within
        /// the machine
        /// </summary>
        [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
        public IResourceManagement ResourceManagement { get; set; }

        /// <summary>
        /// Found recipe providers used to restore recipes links of jobs and processes
        /// </summary>
        [RequiredModuleApi(IsStartDependency = true, IsOptional = true)]
        public ISetupProvider SetupProvider { get; set; }

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
        protected override void OnInitialize()
        {
            Container.RegisterNotifications();

            // Load database model into local container
            Container.ActivateDbContexts(DbContextManager);

            // Register all imported components
            Container.SetInstance(ResourceManagement)
                .SetInstance(ProductManagement);
            // Register optional setup dependency
            if (SetupProvider != null)
                Container.SetInstance(SetupProvider);

            // Register process plugins
            Container.LoadComponents<ICellSelector>();

            // Register plugins for the setup management
            Container.LoadComponents<ISetupTrigger>();
            Container.LoadComponents<IJobScheduler>();

            Container.Resolve<ComponentOrchestration>().Initialize();
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override void OnStart()
        {
            // Activate facade
            ActivateFacade(_jobManagementFacade);
            ActivateFacade(_notificationSourceFacade);
            ActivateFacade(_processControlFacade);

            // Resolve component orchestration and start all components in the correct order
            Container.Resolve<ComponentOrchestration>().Start();
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override void OnStop()
        {
            // Resolve component orchestration and stop all components in the correct order
            Container.Resolve<ComponentOrchestration>().Stop();

            // Deactivate facade
            DeactivateFacade(_jobManagementFacade);
            DeactivateFacade(_notificationSourceFacade);
            DeactivateFacade(_processControlFacade);
        }

#endregion

#region FacadeContainer

        private readonly JobManagementFacade _jobManagementFacade = new();
        IJobManagement IFacadeContainer<IJobManagement>.Facade => _jobManagementFacade;

        private readonly NotificationSourceFacade _notificationSourceFacade = new(ModuleName);
        INotificationSource IFacadeContainer<INotificationSource>.Facade => _notificationSourceFacade;

        private readonly ProcessControlFacade _processControlFacade = new();

        IProcessControl IFacadeContainer<IProcessControl>.Facade => _processControlFacade;

#endregion
    }
}
