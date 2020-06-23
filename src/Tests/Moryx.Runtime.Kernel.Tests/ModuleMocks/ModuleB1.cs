// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks
{
    internal class ModuleB1 : ModuleBase, IFacadeContainer<IFacadeB>
    {
        public ModuleB1()
        {
            Facade = new FacadeB1();
        }

        /// <summary>
        /// Facade controlled by this module
        /// </summary>
        /// <remarks>
        /// The hard-coded name of this property is also used in Moryx.Runtime.Kernel\ModuleManagement\Components\ModuleDependencyManager.cs
        /// </remarks>
        public IFacadeB Facade { get; private set; }
    }

    internal class FacadeB1 : IFacadeB
    {
        
    }
}
