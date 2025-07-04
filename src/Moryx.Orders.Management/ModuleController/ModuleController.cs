// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Products;
using Moryx.ControlSystem.Jobs;
using Moryx.Notifications;
using Moryx.Runtime.Modules;
using System.ComponentModel;
using Moryx.Model;
using Moryx.Orders.Advice;
using Moryx.Orders.Assignment;
using Moryx.Orders.Dispatcher;
using Moryx.Users;
using Moryx.Container;
using Moryx.Configuration;
using Microsoft.Extensions.Logging;
#if COMMERCIAL
using Moryx.ControlSystem;
using Moryx.Logging;
#endif

namespace Moryx.Orders.Management
{
    /// <summary>
    /// The main module class for the OrderManagement.
    /// </summary>
    [Description("Module to handle orders provided by several plugins e.g. Hydra or Web.")]
    public class ModuleController : ServerModuleBase<ModuleConfig>, 
        IFacadeContainer<INotificationSource>, 
        IFacadeContainer<IOrderManagement>
    {
        private const string ModuleName = "OrderManagement";

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;

#region Imports

        /// <summary>
        /// Generic component to manage database contexts
        /// </summary>
        public IDbContextManager DbContextManager { get; }

        /// <summary>
        /// Product management to handle products for the orders
        /// </summary>
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public IProductManagement ProductManagement { get; set; }

        /// <summary>
        /// Job management to handle jobs for the operations
        /// </summary>
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public IJobManagement JobManagement { get; set; }

        /// <summary>
        /// Notification publisher facade to listen to all notifications
        /// </summary>
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public INotificationPublisher NotificationPublisher { get; set; }

        /// <summary>
        /// User management to use users for operations within the order management
        /// </summary>
        [RequiredModuleApi(IsOptional = true, IsStartDependency = true)]
        public IUserManagement UserManagement { get; set; }

#endregion

        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager contextManager) 
            : base(containerFactory, configManager, loggerFactory)
        {
            DbContextManager = contextManager;
        }

        #region State transition

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override void OnInitialize()
        {
            Container.ActivateDbContexts(DbContextManager);

            Container.SetInstance(ProductManagement)
                .SetInstance(JobManagement)
                .SetInstance(UserManagement ?? new NullUserManagement())
                .SetInstance(NotificationPublisher);

            Container.LoadComponents<IProductAssignment>();
            Container.LoadComponents<IPartsAssignment>();
            Container.LoadComponents<IRecipeAssignment>();
            Container.LoadComponents<IOperationValidation>();
            Container.LoadComponents<IDocumentLoader>();

            Container.LoadComponents<ICountStrategy>();
            Container.LoadComponents<IOperationDispatcher>();
            Container.LoadComponents<IAdviceExecutor>();
#if COMMERCIAL
            if (LicenseCheck.IsDeveloperLicense())
                Logger.Log(LogLevel.Warning, "Running with developer license for 1h");
#endif
        }

            /// <inheritdoc />
        protected override void OnStart()
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                throw new InvalidOperationException("No license available!");
#endif
            if (Config.Users.UserRequired && UserManagement is NullUserManagement)
                throw new InvalidOperationException("UserRequired configured but there is no UserManagement module available");

            Container.RegisterNotifications();
            ActivateFacade(_notificationSourceFacade);

            Container.Resolve<ComponentOrchestration>().Start();

            ActivateFacade(_orderManagementFacade);
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            DeactivateFacade(_notificationSourceFacade);
            DeactivateFacade(_orderManagementFacade);

            Container.Resolve<ComponentOrchestration>().Stop();
        }

#endregion

        private readonly NotificationSourceFacade _notificationSourceFacade = new(ModuleName);

        INotificationSource IFacadeContainer<INotificationSource>.Facade => _notificationSourceFacade;

        private readonly OrderManagementFacade _orderManagementFacade = new();

        IOrderManagement IFacadeContainer<IOrderManagement>.Facade => _orderManagementFacade;
    }
}

