// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks;

internal class ModuleB2 : ModuleBase, IFacadeContainer<IFacadeB>
{
    public ModuleB2()
    {
        Facade = new FacadeB();
    }

    /// <summary>
    /// Facade controlled by this module
    /// </summary>
    /// <remarks>
    /// The hard-coded name of this property is also used in Moryx.Runtime.Kernel\ModuleManagement\Components\ModuleDependencyManager.cs
    /// </remarks>
    public IFacadeB Facade { get; private set; }
}