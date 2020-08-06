// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Threading;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;
using Moryx.TestModule;

namespace Moryx.DependentTestModule
{
    [ServerModule(ModuleName)]
    [Description("Test module for System tests")]
    public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>, IFacadeContainer<IDependentTestModule>
    {
        public const string ModuleName = "DependentTestModule";

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;

        [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
        public ITestModule TestModule { get; set; }

        #region State transition
        // ReSharper disable RedundantOverridenMember
        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override void OnInitialize()
        {
            Container.LoadComponents<ISimpleHelloWorldWcfConnector>();
        }

        private ISimpleHelloWorldWcfConnector _connector;

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override void OnStart()
        {
            Thread.Sleep(2000); // Just for system testing.

            var factory = Container.Resolve<ISimpleHelloWorldWcfConnectorFactory>();
            _connector = factory.Create(Config.SimpleHelloWorldWcfConnector);

            _connector.Initialize(Config.SimpleHelloWorldWcfConnector);
            _connector.Start();

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

            // Stop connector
            _connector.Stop();
            _connector = null;
        }
        #endregion

        #region FacadeContainer
        private readonly DependentTestModuleFacade _testModuleFacade = new DependentTestModuleFacade();
        IDependentTestModule IFacadeContainer<IDependentTestModule>.Facade => _testModuleFacade;

        #endregion
    }
}
