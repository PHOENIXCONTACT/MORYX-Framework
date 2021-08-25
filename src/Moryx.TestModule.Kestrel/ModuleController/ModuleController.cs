// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Threading;
using Moryx.Communication.Endpoints;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;
using Moryx.TestModule.Kestrel.Controllers;

namespace Moryx.TestModule.Kestrel.ModuleController
{
    [ServerModule(ModuleName)]
    [Description("Test module for System tests")]
    public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>
    {
        private IEndpointHost _host;
        public const string ModuleName = "KestrelTester";

        #region Dependencies
        
        /// <summary>
        /// Host factory to create wcf services
        /// </summary>
        public IEndpointHosting Hosting { get; set; }

        #endregion

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;

        #region State transition

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            Container.ActivateHosting(Hosting);
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            var hostFactory = Container.Resolve<IEndpointHostFactory>();
            _host = hostFactory.CreateHost(typeof(TestController), null);
            _host.Start();
        }

        /// <inheritdoc />

        protected override void OnStop()
        {
            _host.Stop();
        }
        #endregion
    }
}
