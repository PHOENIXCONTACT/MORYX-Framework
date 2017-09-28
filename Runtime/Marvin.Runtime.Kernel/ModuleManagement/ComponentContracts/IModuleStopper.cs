using Marvin.Modules.Server;

namespace Marvin.Runtime.Kernel.ModuleManagement
{
    internal interface IModuleStopper : IModuleManagerComponent
    {
        /// <summary>
        /// Stop this plugin and all required dependencies
        /// </summary>
        /// <param name="module"></param>
        void Stop(IServerModule module);

        /// <summary>
        /// Stop all services
        /// </summary>
        void StopAll();
    }
}
