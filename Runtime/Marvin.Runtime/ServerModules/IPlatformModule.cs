using Marvin.Modules.Server;
using Marvin.Runtime.ModuleManagement;

namespace Marvin.Runtime.ServerModules
{
    /// <summary>
    /// Module interface for all platform extension modules
    /// </summary>
    public interface IPlatformModule : IServerModule
    {
        /// <summary>
        /// Module manager reference filled by the module manager himself
        /// </summary>
        void SetModuleManager(IModuleManager moduleManager);
    }
}