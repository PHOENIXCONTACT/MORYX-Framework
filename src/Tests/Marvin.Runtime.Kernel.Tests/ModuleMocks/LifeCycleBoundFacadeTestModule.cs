// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Runtime.Kernel.Tests.Dummys;
using Marvin.Runtime.Kernel.Tests.ModuleMocks.Facade;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel.Tests.ModuleMocks
{
    public class LifeCycleBoundFacadeTestModule : ServerModuleFacadeControllerBase<RuntimeConfigManagerTestConfig2>
    {
        public override string Name => "LifeCycleBoundTestModule";

        private TestFacade _facade;

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
