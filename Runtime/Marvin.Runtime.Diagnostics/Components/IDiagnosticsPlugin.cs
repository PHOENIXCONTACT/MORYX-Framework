using Marvin.Modules.ModulePlugins;

namespace Marvin.Runtime.Diagnostics
{
    /// <summary>
    /// Interface for a diagnostic plugin.
    /// </summary>
    public interface IDiagnosticsPlugin : IConfiguredModulePlugin<DiagnosticsPluginConfigBase>
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        string Name { get; }
    }
}
