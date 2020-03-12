// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Logging;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
{
    internal class ModuleInitializer : ModuleManagerComponent, IModuleInitializer
    {
        public IModuleLogger Logger { get; }

        public ModuleInitializer(IModuleLogger logger)
        {
            Logger = logger;
        }

        public void Initialize(IServerModule module)
        {
            module.Initialize();
        }
    }
}
