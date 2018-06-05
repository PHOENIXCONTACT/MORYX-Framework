using Marvin.Modules;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Strategy applied to decide wether or not to restart a plugin
    /// </summary>
    internal interface IModuleFailureStrategy
    {
        /// <summary>
        /// Decide if failed plugin shall be restarted, queued or stopped
        /// </summary>
        bool ReincarnateOnFailure(IModule module);
    }
}
