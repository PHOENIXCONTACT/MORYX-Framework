// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks;

internal class ModuleC : ModuleBase
{
    [RequiredModuleApi(IsStartDependency = true)]
    public IFacadeB[] Facades { get; set; }
}

internal class ModuleCSingle : ModuleBase
{
    [RequiredModuleApi(IsStartDependency = true)]
    public IFacadeB Facades { get; set; }
}