using System;

namespace Marvin.Modules.ModulePlugins
{
    /// <summary>
    /// Base interface for all components collaborating within an <see cref="IModule"/> composition. 
    /// For increased flexibility and extensibility modules should be designed as compositions of exchangeable plugins. 
    /// Application specific behaviour and patterns like strategy or CoR pattern should include plugins. 
    /// Tailored to the different requirements IModulePlugins come in different variations.
    /// To provide fully restartable modules each plugin must be fully disposable.
    /// </summary>
    public interface IModulePlugin : IDisposable
    {
        /// <summary>
        /// Start internal execution of active and/or periodic functionality.
        /// </summary>
        void Start();
    }
}
