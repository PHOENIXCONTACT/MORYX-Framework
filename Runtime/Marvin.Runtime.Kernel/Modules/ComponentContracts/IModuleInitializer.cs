using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Component handling the initialize of a module
    /// </summary>
    internal interface IModuleInitializer
    {
        /// <summary>
        /// Initializes a module
        /// </summary>
        void Initialize(IServerModule module);
    }
}