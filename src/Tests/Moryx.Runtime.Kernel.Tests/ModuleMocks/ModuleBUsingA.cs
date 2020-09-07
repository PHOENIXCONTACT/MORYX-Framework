// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks
{
    internal class ModuleBUsingA : ModuleBase, IFacadeContainer<IFacadeB>
    {
        public ModuleBUsingA()
        {
            Facade = new FacadeB();
        }

        [RequiredModuleApi(IsStartDependency = true)]
        public IFacadeA Dependency { get; set; }

        /// <summary>
        /// Facade controlled by this module
        /// </summary>
        public IFacadeB Facade { get; }
    }
}