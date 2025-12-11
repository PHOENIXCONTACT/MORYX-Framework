// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Kernel.Tests.Dummies;
using Moryx.Runtime.Kernel.Tests.ModuleMocks.Facade;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks
{
    public class LifeCycleBoundFacadeTestModule : ServerModuleBase<RuntimeConfigManagerTestConfig2>
    {
        public override string Name => "LifeCycleBoundTestModule";

        private TestFacade _facade;

        public LifeCycleBoundFacadeTestModule(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
            : base(containerFactory, configManager, loggerFactory)
        {
        }

        public int ActivatedCount { get; private set; }
        public int DeactivatedCount { get; private set; }

        public FacadeBase Facade => _facade;

        protected override Task OnInitializeAsync()
        {
            _facade = new TestFacade();
            _facade.StateChanged += OnFacadeActivated;

            return Task.CompletedTask;
        }

        protected override Task OnStartAsync()
        {
            ActivateFacade(_facade);
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync()
        {
            DeactivateFacade(_facade);
            return Task.CompletedTask;
        }

        private void OnFacadeActivated(object sender, bool activated)
        {
            if (activated)
                ++ActivatedCount;
            else
                ++DeactivatedCount;
        }
    }
}
