// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks
{
    internal class ModuleA : ModuleBase, IFacadeContainer<IFacadeA>
    {
        public ModuleA()
        {
            Facade = new FacadaA();
        }

        /// <summary>
        /// Facade controlled by this module
        /// </summary>
        /// <remarks>
        /// The hard-coded name of this property is also used in Moryx.Runtime.Kernel\ModuleManagement\Components\ModuleDependencyManager.cs
        /// </remarks>
        public IFacadeA Facade { get; private set; }
    }

    internal class FacadaA : IFacadeA
    {
    }
}
