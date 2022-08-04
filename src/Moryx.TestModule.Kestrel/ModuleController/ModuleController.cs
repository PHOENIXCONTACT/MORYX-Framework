// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.Communication.Endpoints;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;

namespace Moryx.TestModule.Kestrel
{
    [ServerModule(ModuleName)]
    [Description("Test module for testing kestrel services")]
    public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>, IFacadeContainer<ITestFacade>
    {
        private IEndpointHost _host;
        public const string ModuleName = "TestModuleKestrel";

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;

        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, IServerLoggerManagement loggerManagement) 
            : base(containerFactory, configManager, loggerManagement)
        {
        }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
        }

        /// <inheritdoc />

        protected override void OnStop()
        {
        }


        #region FacadeContainer

        private readonly TestFacade _testModule = new TestFacade();

        ITestFacade IFacadeContainer<ITestFacade>.Facade => _testModule;

        #endregion
    }
}
