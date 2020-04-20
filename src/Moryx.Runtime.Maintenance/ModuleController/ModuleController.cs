// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.Logging;
using Moryx.Model;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Container;
using Moryx.Runtime.Maintenance.Contracts;
using Moryx.Runtime.Maintenance.Plugins;
using Moryx.Runtime.Modules;
using Moryx.Tools.Wcf.FileSystem;

namespace Moryx.Runtime.Maintenance
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

        #endregion

        #region Dependency Injection

        /// <summary>
        /// Model configurators. Injected by castle.
        /// </summary>
        public IDbContextFactory DbContextFactory { get; set; }

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

            Container.SetInstance(DbContextFactory);

            Container.LoadComponents<IMaintenancePlugin>();
        }

        /// <summary>
        /// Called when [start].
        /// </summary>
        /// <exception cref="System.Exception">Failed to start module  + moduleConfig.PluginName</exception>
        protected override void OnStart()
        {
            var pluginFac = Container.Resolve<IMaintenancePluginFactory>();
            var plugins = Container.ResolveAll<IMaintenancePlugin>().ToList();

            var pluginConfigs = Config.Plugins.Distinct().ToArray();

            var configuredPlugins = pluginConfigs.Select(pluginConfig => pluginFac.Create(pluginConfig)).ToList();
            var unconfiguredPlugins = plugins.Except(configuredPlugins).ToArray();

            foreach (var unconfiguredPlugin in unconfiguredPlugins)
            {
                var baseType = unconfiguredPlugin.GetType().BaseType;
                if (baseType == null || !typeof(MaintenancePluginBase<,>).IsAssignableFrom(baseType.GetGenericTypeDefinition()))
                    throw new ArgumentException("MaintenancePlugins should be of type MaintenancePluginBase");
                
                var configType = baseType.GetGenericArguments()[0];

                var pluginConfig = (MaintenancePluginConfig)Activator.CreateInstance(configType);

                Config.Plugins.Add(pluginConfig);

                var instance = pluginFac.Create(pluginConfig);
                configuredPlugins.Add(instance);
            }

            foreach (var plugin in plugins)
            {
                try
                {
                    plugin.Start();
                }
                catch (Exception ex)
                {
                    var pluginName = plugin.GetType().Name;
                    Logger.LogException(LogLevel.Error, ex, "Failed to start plugin {0}", pluginName);
                    throw new Exception("Failed to start plugin " + pluginName, ex);
                }
            }
        }

        /// <summary>
        /// Called when [stop].
        /// </summary>
        protected override void OnStop()
        {

        }
    }
}
