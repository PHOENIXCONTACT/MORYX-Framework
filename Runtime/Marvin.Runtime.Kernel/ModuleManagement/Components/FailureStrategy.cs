using Marvin.Modules;
using Marvin.Runtime.ServerModules;

namespace Marvin.Runtime.Kernel
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
