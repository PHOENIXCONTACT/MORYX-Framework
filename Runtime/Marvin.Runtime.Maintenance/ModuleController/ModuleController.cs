using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.Logging;
using Marvin.Model;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Container;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Runtime.Modules;
using Marvin.Tools.Wcf;
using Marvin.Tools.Wcf.FileSystem;

namespace Marvin.Runtime.Maintenance
{
    /// <summary>
    /// Maintenance module that hosts the plugins.
    /// </summary>
    [ServerModule(ModuleName)]
    public class ModuleController : ServerModuleBase<ModuleConfig>, IPlatformModule
    {
        internal const string ModuleName = "Maintenance";

        #region Fields

        private IModuleManager _moduleManager;

        private readonly List<IConfiguredServiceHost> _serviceHosts = new List<IConfiguredServiceHost>();
        
        #endregion

        #region Dependency Injection

        /// <summary>
        /// Model configurators. Injected by castle.
        /// </summary>
        public IUnitOfWorkFactory[] UnitOfWorkFactories { get; set; }

        /// <summary>
        /// runtime config manager. Injected by castle.
        /// </summary>
        public IRuntimeConfigManager RuntimeConfigManager { get; set; }

        /// <summary>
        /// Set the module manager. Not injected by castle.
        /// </summary>
        /// <param name="moduleManager">the module manager.</param>
        public void SetModuleManager(IModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }

        #endregion

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;

        /// <summary>
        /// Called when [initialize].
        /// </summary>
        protected override void OnInitialize()
        {
            Container.Register<IPolicyRetriever, PolicyRetriever>()
                .SetInstance(_moduleManager).SetInstance(RuntimeConfigManager)
                .SetInstance((IServerLoggerManagement)LoggerManagement);

            // Register all unit of work factories
            foreach (var factory in UnitOfWorkFactories)
                Container.SetInstance(factory);

            Container.LoadComponents<IMaintenancePlugin>();
        }

        /// <summary>
        /// Called when [start].
        /// </summary>
        /// <exception cref="System.Exception">Failed to start module  + moduleConfig.PluginName</exception>
        protected override void OnStart()
        {
            // Start plugins
            var pluginFac = Container.Resolve<IMaintenancePluginFactory>();
            foreach (var pluginConfig in Config.Plugins.Where(module => module.IsActive).Distinct())
            {
                var plugin = pluginFac.Create(pluginConfig);
                try
                {
                    plugin.Start();
                }
                catch (Exception ex)
                {
                    Logger.LogException(LogLevel.Error, ex, "Failed to start plugin {0}", pluginConfig.PluginName);
                    throw new Exception("Failed to start plugin " + pluginConfig.PluginName, ex);
                }
            }
        }

        /// <summary>
        /// Called when [stop].
        /// </summary>
        protected override void OnStop()
        {
            foreach (var serviceHost in _serviceHosts)
            {
                serviceHost.Dispose();
            }
            _serviceHosts.Clear();
        }

    }
}