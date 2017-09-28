using Marvin.Configuration;
using Marvin.Container;
using Marvin.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marvin.Runtime.Diagnostics
{
    /// <summary>
    /// Helper class to start and stop Diagnostic plugins with the IsActive flag.
    /// </summary>
    [Component(LifeCycle.Singleton, Name = PluginName)]
    internal class DiagnosticControllerHelper : IDisposable
    {
        /// <summary>
        /// Const name of the plugin.
        /// </summary>
        internal const string PluginName = "DiagnosticControllerHelper";
        /// <summary>
        /// Factory to create and start plugins.
        /// </summary>
        public IDiagnosticsPluginFactory Factory { get; set; }

        /// <summary>
        /// List of the current active running plugins.
        /// </summary>
        private readonly List<IDiagnosticsPlugin> _runningDiagnosticsPlugins = new List<IDiagnosticsPlugin>();

        /// <summary>
        /// Injected module to report an error when it occures.
        /// </summary>
        public IModuleErrorReporting ModuleErrorReporting { get; set; }

        /// <summary>
        /// Config of the module to get the list of the plugins.
        /// </summary>
        public ModuleConfig Config { get; set; }

        /// <summary>
        /// Code executed after OnInitialize. Starts all plugins fetched from the config which have IsAcive = true.
        /// All the others are not even initialized.
        /// </summary>
        public void OnStart()
        {        
            foreach (var pluginConfig in Config.Plugins)
            {
                try
                {
                    if (pluginConfig.IsActive)
                    {
                        StartPlugin(pluginConfig);
                    }
                    pluginConfig.ConfigChanged += OnConfigChanged;
                }
                catch (Exception e)
                {
                    ModuleErrorReporting.ReportWarning(this, e);
                }
            }
        }

        /// <summary>
        /// When the property "IsActive" was changed, then will the start or stop plugin method be called.
        /// </summary>
        private void OnConfigChanged(object sender, ConfigChangedEventArgs e)
        {
            DiagnosticsPluginConfigBase changedConfig = sender as DiagnosticsPluginConfigBase;
            if (changedConfig == null)
            {
                return;
            }

            if (!e.Contains(() => changedConfig.IsActive))
            {
                return;
            }

            if (changedConfig.IsActive)
            {
                StartPlugin(changedConfig);
            }
            else
            {
                StopPlugin(changedConfig);
            }
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        public void OnStop()
        {
            foreach (DiagnosticsPluginConfigBase pluginConfig in Config.Plugins)
            {
                pluginConfig.ConfigChanged -= OnConfigChanged;
            }
            _runningDiagnosticsPlugins.Clear();
        }

        /// <summary>
        /// Starts the wanted plugin and add it to the list with the running plugins.
        /// </summary>
        /// <param name="pluginConfig">The config of the current plugin.</param>
        private void StartPlugin(DiagnosticsPluginConfigBase pluginConfig)
        {
            IDiagnosticsPlugin plugin = Factory.Create(pluginConfig);
            plugin.Start();
            _runningDiagnosticsPlugins.Add(plugin);
        }

        /// <summary>
        /// Stops the wanted plugin and remove it from the list of the running plugins.
        /// </summary>
        /// <param name="pluginConfig">The config of the current plugin.</param>
        private void StopPlugin(DiagnosticsPluginConfigBase pluginConfig)
        {
            IDiagnosticsPlugin plugin =_runningDiagnosticsPlugins.First(diagnosticsPlugin => diagnosticsPlugin.Name == pluginConfig.PluginName);
            _runningDiagnosticsPlugins.Remove(plugin);
            plugin.Dispose();           
        }

        /// <summary>
        /// Disposes this class and unwire all events.
        /// </summary>
        public void Dispose()
        {
            foreach (var pluginConfig in Config.Plugins)
            {
                 pluginConfig.ConfigChanged -= OnConfigChanged;
            }
            _runningDiagnosticsPlugins.Clear();
        }
    }
}
