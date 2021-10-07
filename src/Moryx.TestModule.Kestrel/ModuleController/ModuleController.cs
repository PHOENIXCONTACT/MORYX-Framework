// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.Communication.Endpoints;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;

namespace Moryx.TestModule.Kestrel
{
    [ServerModule(ModuleName)]
    [Description("Test module for testing kestrel services")]
    public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>
    {
        private IEndpointHost _host;
        public const string ModuleName = "TestModuleKestrel";

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
    }
}
