// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks
{
    internal class ModuleB3 : ModuleBase, IFacadeContainer<IFacadeB>
    {
        public ModuleB3()
        {
            Facade = new FacadeB();
        }

        /// <summary>
        /// Facade controlled by this module
        /// </summary>
        public IFacadeB Facade { get; }
    }
}
