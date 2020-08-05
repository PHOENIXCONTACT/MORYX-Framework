// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading;
using Moryx.Model;
using Moryx.Model.Repositories;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;
using Moryx.TestTools.Test.Model;

namespace Moryx.TestModule
{
    [ServerModule(ModuleName)]
    public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>, IFacadeContainer<ITestModule>
    {
        public const string ModuleName = "TestModule";

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;

        /// <summary>
        /// Db context factory for data models
        /// </summary>
        public IDbContextManager DbContextManager { get; set; }

        private IHelloWorldWcfConnector _connector;

        #region State transition
        // ReSharper disable RedundantOverridenMember
        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override void OnInitialize()
        {
            Container.ActivateDbContexts(DbContextManager);

            Container.LoadComponents<IHelloWorldWcfConnector>();
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
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

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
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
