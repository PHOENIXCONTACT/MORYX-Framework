// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel.Tests.ModuleMocks
{
    internal class ModuleADependend : ModuleBase
    {
        [RequiredModuleApi]
        public IFacadeA Dependency { get; set; }
    }
}
