// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
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
        protected override Task OnInitializeAsync()
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
        protected override async Task OnStartAsync()
        {
            // Start Manager
            Container.Resolve<IProductStorage>().Start();
            await Container.Resolve<IProductManager>().StartAsync();

            // Activate facades
            ActivateFacade(_productManagement);
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override Task OnStopAsync()
        {
            // Deactivate facades
            DeactivateFacade(_productManagement);

            Container.Resolve<IProductStorage>().Stop();
            return Container.Resolve<IProductManager>().StopAsync();
        }

        private readonly ProductManagementFacade _productManagement = new();

        IProductManagement IFacadeContainer<IProductManagement>.Facade => _productManagement;
        IWorkplans IFacadeContainer<IWorkplans>.Facade => _productManagement;
    }

}
