// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Modules;
using System;
using System.Threading.Tasks;
using Moryx.Runtime.Kernel.Tests.Dummies;

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

        protected override Task OnInitializeAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task OnStartAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task OnStopAsync()
        {
            throw new NotImplementedException();
        }
    }
}
