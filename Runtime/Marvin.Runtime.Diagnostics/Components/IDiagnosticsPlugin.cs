using System;
using Marvin.Modules;

namespace Marvin.Runtime.Diagnostics
{
    /// <summary>
    /// Interface for a diagnostic plugin.
    /// </summary>
    public interface IDiagnosticsPlugin : IConfiguredPlugin<DiagnosticsPluginConfigBase>, IDisposable
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        string Name { get; }
    }
}
