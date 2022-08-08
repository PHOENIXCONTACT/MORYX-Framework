// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Kernel.Tests.Dummys;
using Moryx.Runtime.Kernel.Tests.ModuleMocks.Facade;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks
{
    public class LifeCycleBoundFacadeTestModule : ServerModuleFacadeControllerBase<RuntimeConfigManagerTestConfig2>
    {
        public override string Name => "LifeCycleBoundTestModule";

        private TestFacade _facade;

        public LifeCycleBoundFacadeTestModule(IModuleContainerFactory containerFactory, IConfigManager configManager, IServerLoggerManagement loggerManagement) 
            : base(containerFactory, configManager, loggerManagement)
        {
        }

        public int ActivatedCount { get; private set; }
        public int DeactivatedCount { get; private set; }

        public FacadeBase Facade => _facade;

        protected override void OnInitialize()
        {
            _facade = new TestFacade();
            _facade.StateChanged += OnFacadeActivated;
        }

        protected override void OnStart()
        {
            ActivateFacade(_facade);
        }

        protected override void OnStop()
        {
            DeactivateFacade(_facade);
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
