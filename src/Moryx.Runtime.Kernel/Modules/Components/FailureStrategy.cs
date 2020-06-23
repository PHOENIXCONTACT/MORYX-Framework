// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    internal class FailureStrategy : IModuleFailureStrategy
    {
        private readonly ModuleManagerConfig _config;

        public FailureStrategy(ModuleManagerConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Decide if failed plugin shall be restarted, queued or stopped
        /// </summary>
        public bool ReincarnateOnFailure(IModule module)
        {
            return _config.GetOrCreate(module.Name).FailureBehaviour.HasFlag(FailureBehaviour.Reincarnate);
        }
    }
}
