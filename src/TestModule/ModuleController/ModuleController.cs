// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Threading;
using Moryx.Model;
using Moryx.Model.Repositories;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;
using Moryx.Runtime.Wcf;
using Moryx.TestTools.Test.Model;
using Moryx.Tools.Wcf;

namespace Moryx.TestModule
{
    [ServerModule(ModuleName)]
    [Description("Test module for System tests")]
    public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>, IFacadeContainer<ITestModule>
    {
        public const string ModuleName = "TestModule";

        #region Dependencies

        [Named(TestModelConstants.Namespace)]
        public IUnitOfWorkFactory TestFactory { get; set; }

        /// <summary>
        /// Host factory to create wcf services
        /// </summary>
        public IWcfHostFactory WcfHostFactory { get; set; }

        #endregion

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;
        private IHelloWorldWcfConnector _connector;

        #region State transition

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            Container.RegisterWcf(WcfHostFactory, Logger);
            Container.ActivateDbContexts(DbContextManager);

            Container.LoadComponents<IHelloWorldWcfConnector>();
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            Logger.Log(Config.LogLevel, "Sending log message with level '{0}'", Config.LogLevel);

            Thread.Sleep(Config.SleepTime); // Just for system testing.

            var factory = Container.Resolve<IHelloWorldWcfConnectorFactory>();

            _connector = factory.Create(Config.HelloWorldWcfConnector);
            _connector.Initialize(Config.HelloWorldWcfConnector);
            _connector.Start();

            // Activate facades
            ActivateFacade(_testModule);
        }

        /// <inheritdoc />

        protected override void OnStop()
        {
            Thread.Sleep(Config.SleepTime); // Just for system testing.

            // Deactivate facades
            DeactivateFacade(_testModule);

            // Stop connector
            _connector.Stop();
            _connector = null;
        }
        #endregion

        #region FacadeContainer

        private readonly TestModuleFacade _testModule = new TestModuleFacade();

        ITestModule IFacadeContainer<ITestModule>.Facade => _testModule;

        #endregion
    }
}
