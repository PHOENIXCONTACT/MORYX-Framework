// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model;
using Moryx.Runtime.Modules;

namespace Moryx.Workplans.Editing
{
    /// <summary>
    /// Workplan Editor server controller
    /// </summary>
    public class ModuleController : ServerModuleBase<ModuleConfig>, IFacadeContainer<Workplans.IWorkplanEditing>
    {
        internal const string ModuleName = "WorkplanEditing";

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;

        /// <summary>
        /// Generic component to load data models
        /// </summary>
        public IDbContextManager DbContextManager { get; set; }

        /// <inheritdoc/>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager dbContextManager) : base(containerFactory, configManager, loggerFactory)
        {
            DbContextManager = dbContextManager;
        }

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Container.ActivateDbContexts(DbContextManager);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Code executed after OnInitialize
        /// </summary>
        protected override Task OnStartAsync(CancellationToken cancellationToken)
        {
            Container.Resolve<IWorkplanEditor>().Start();

            ActivateFacade(_facade);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Code executed when service is stopped
        /// </summary>
        protected override Task OnStopAsync(CancellationToken cancellationToken)
        {
            DeactivateFacade(_facade);
            return Task.CompletedTask;
        }

        private readonly WorkplanFacade _facade = new();

        Workplans.IWorkplanEditing IFacadeContainer<Workplans.IWorkplanEditing>.Facade => _facade;
    }
}
