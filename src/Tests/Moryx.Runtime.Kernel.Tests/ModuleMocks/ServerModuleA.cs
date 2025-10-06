// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Kernel.Tests.Dummys;
using Moryx.Runtime.Modules;
using System;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks
{
    public class ServerModuleA : ServerModuleBase<RuntimeConfigManagerTestConfig2>, IFacadeContainer<IFacadeA>

    {
        public ServerModuleA(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory) : base(containerFactory, configManager, loggerFactory)
        {
            Facade = new FacadaA();
        }

        public override string Name => "ServerModuleA";

        private readonly FacadaA _aFacade = new();
        IFacadeA IFacadeContainer<IFacadeA>.Facade => _aFacade;
        public IFacadeA Facade { get; private set; }

        protected override void OnInitialize()
        {
            throw new NotImplementedException();
        }

        protected override void OnStart()
        {
            throw new NotImplementedException();
        }

        protected override void OnStop()
        {
            throw new NotImplementedException();
        }
    }
}
