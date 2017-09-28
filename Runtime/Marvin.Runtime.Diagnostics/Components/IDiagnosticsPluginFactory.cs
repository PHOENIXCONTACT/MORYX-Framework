using Marvin.Container;

namespace Marvin.Runtime.Diagnostics
{
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface IDiagnosticsPluginFactory
    {
        /// <summary>
        /// Create a diagnostics module with this name
        /// </summary>
        IDiagnosticsPlugin Create(DiagnosticsPluginConfigBase config);

        /// <summary>
        /// Destroy this instance
        /// Almost never used!
        /// </summary>
        void Destroy(IDiagnosticsPlugin instance);
    }
}
