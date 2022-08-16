// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moryx.Communication.Endpoints;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Model;
using Moryx.Runtime;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;

namespace Moryx.TestModule
{
    [Description("Test module for System tests")]
    public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>, IFacadeContainer<ITestModule>
    {
        #region Dependencies

        /// <summary>
        /// Db context factory for data models
        /// </summary>
        public IDbContextManager DbContextManager { get; set; }

        #endregion

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => "TestModule";


        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory) 
            : base(containerFactory, configManager, loggerFactory)
        {
        }


        #region State transition

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            Container.ActivateDbContexts(DbContextManager);
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            Thread.Sleep(Config.SleepTime); // Just for system testing.

            // Activate facades
            ActivateFacade(_testModule);
        }

        /// <inheritdoc />

        protected override void OnStop()
        {
            Thread.Sleep(Config.SleepTime); // Just for system testing.

            // Deactivate facades
            DeactivateFacade(_testModule);
        }
        #endregion

        #region FacadeContainer

        private readonly TestModuleFacade _testModule = new TestModuleFacade();

        ITestModule IFacadeContainer<ITestModule>.Facade => _testModule;

        #endregion
    }
}
