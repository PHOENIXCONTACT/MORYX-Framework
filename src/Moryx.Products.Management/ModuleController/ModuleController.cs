// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Products;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model;
using Moryx.Runtime.Modules;
using Moryx.Workplans;

namespace Moryx.Products.Management
{
    /// <summary>
    /// The main controller of all product modules.
    /// </summary>
    public class ModuleController : ServerModuleBase<ModuleConfig>,
        IFacadeContainer<IProductManagement>,
        IFacadeContainer<IWorkplans>
    {
        internal const string ModuleName = "ProductManager";
        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;

        /// <summary>
        /// Generic component to access every data model
        /// </summary>
        public IDbContextManager DbContextManager { get; }

        /// <summary>
        /// Create new module instance
        /// </summary>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager contextManager)
            : base(containerFactory, configManager, loggerFactory)
        {
            DbContextManager = contextManager;
        }

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            // Extend container
            Container
                .ActivateDbContexts(DbContextManager);

            // Register imports
            Container.SetInstance(ConfigManager);

            // Load all product plugins
            Container.LoadComponents<IProductStorage>();
            Container.LoadComponents<IProductImporter>();

            // Load strategies
            Container.LoadComponents<IProductTypeStrategy>();
            Container.LoadComponents<IProductInstanceStrategy>();
            Container.LoadComponents<IProductLinkStrategy>();
            Container.LoadComponents<IProductRecipeStrategy>();
            Container.LoadComponents<IPropertyMapper>();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        protected override async Task OnStartAsync(CancellationToken cancellationToken)
        {
            // Start Manager
            await Container.Resolve<IProductStorage>().StartAsync(cancellationToken);
            await Container.Resolve<IProductManager>().StartAsync(cancellationToken);

            // Activate facades
            ActivateFacade(_productManagement);
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        protected override async Task OnStopAsync(CancellationToken cancellationToken)
        {
            // Deactivate facades
            DeactivateFacade(_productManagement);

            await Container.Resolve<IProductStorage>().StopAsync(cancellationToken);
            await Container.Resolve<IProductManager>().StopAsync(cancellationToken);
        }

        private readonly ProductManagementFacade _productManagement = new();

        IProductManagement IFacadeContainer<IProductManagement>.Facade => _productManagement;
        IWorkplans IFacadeContainer<IWorkplans>.Facade => _productManagement;
    }

}
