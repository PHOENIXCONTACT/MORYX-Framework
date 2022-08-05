// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Threading;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;
using Moryx.TestModule;

namespace Moryx.DependentTestModule
{
    [Description("Test module for System tests")]
    public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>, IFacadeContainer<IDependentTestModule>
    {
        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => "DependentTestModule";

        [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
        public ITestModule TestModule { get; set; }

        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, IServerLoggerManagement loggerManagement) 
            : base(containerFactory, configManager, loggerManagement)
        {
        }

        #region State transition

        // ReSharper disable RedundantOverridenMember
        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override void OnInitialize()
        {
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override void OnStart()
        {
            Thread.Sleep(2000); // Just for system testing.

            // Activate facades
            ActivateFacade(_testModuleFacade);
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override void OnStop()
        {
            Thread.Sleep(2000); // Just for system testing.

            // Deactivate facades
            DeactivateFacade(_testModuleFacade);
        }

        #endregion

        #region FacadeContainer
        private readonly DependentTestModuleFacade _testModuleFacade = new DependentTestModuleFacade();

        IDependentTestModule IFacadeContainer<IDependentTestModule>.Facade => _testModuleFacade;

        #endregion
    }
}
