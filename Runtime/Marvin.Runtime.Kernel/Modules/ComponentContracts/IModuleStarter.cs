using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Component handling the start of modules
    /// </summary>
    internal interface IModuleStarter : IModuleManagerComponent
    {
        /// <summary>
        /// Initialize this module to try to achieve ready state
        /// </summary>
        void Initialize(IServerModule module);

        /// <summary>
        /// Start a plugin and start dependencies if necessary
        /// </summary>
        void Start(IServerModule module);

        /// <summary>
        /// Start all services
        /// </summary>
        void StartAll();
    }
}
