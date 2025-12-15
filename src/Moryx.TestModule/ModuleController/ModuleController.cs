// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model;
using Moryx.Runtime.Modules;

namespace Moryx.TestModule
{
    [Description("Test module for System tests")]
    public class ModuleController : ServerModuleBase<ModuleConfig>, IFacadeContainer<ITestModule>
    {
        /// <summary>
        /// Db context factory for data models
        /// </summary>
        public IDbContextManager ContextManager { get; set; }

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => "TestModule";

        public ModuleController(IOptions<ModuleConfig> options, IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager contextManager)
            : base(containerFactory, configManager, loggerFactory)
        {
            ContextManager = contextManager;

            var config = options.Value;
        }

        #region State transition

        /// <inheritdoc />
        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Container.ActivateDbContexts(ContextManager);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override async Task OnStartAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(Config.SleepTime, cancellationToken); // Just for system testing.

            var plugin = Container.Resolve<ITestPlugin>("TestPlugin");
            plugin.Start();

            // Activate facades
            ActivateFacade(_testModule);
        }

        /// <inheritdoc />
        protected override async Task OnStopAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(Config.SleepTime, cancellationToken); // Just for system testing.

            // Deactivate facades
            DeactivateFacade(_testModule);
        }
        #endregion

        #region FacadeContainer

        private readonly TestModuleFacade _testModule = new();

        ITestModule IFacadeContainer<ITestModule>.Facade => _testModule;

        #endregion
    }
}
