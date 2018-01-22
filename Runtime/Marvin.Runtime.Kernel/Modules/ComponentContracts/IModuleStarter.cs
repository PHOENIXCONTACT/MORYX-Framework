using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Component handling the start of modules
    /// </summary>
    internal interface IModuleStarter : IModuleManagerComponent
    {
        /// <summary>
        /// Starts a module and all dependencies if necessary
        /// </summary>
        void Start(IServerModule module);

        /// <summary>
        /// Starts all modules
        /// </summary>
        void StartAll();
    }
}
