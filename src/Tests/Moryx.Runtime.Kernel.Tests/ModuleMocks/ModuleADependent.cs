// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks;

internal class ModuleADependent : ModuleBase, IFacadeContainer<IFacadeC>
{
    [RequiredModuleApi(IsStartDependency = true)]
    public IFacadeA Dependency { get; set; }

    public IFacadeC Facade { get; set; } = new FacadeC();
}

internal class ModuleADependentTransient : ModuleBase
{
    [RequiredModuleApi(IsStartDependency = true)]
    public IFacadeC Dependency { get; set; }
}