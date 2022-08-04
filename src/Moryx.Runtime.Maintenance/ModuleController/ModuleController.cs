// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using System.Linq;
using Moryx.Communication.Endpoints;
using Moryx.Logging;
using Moryx.Model;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;
using Moryx.Container;
using Moryx.Configuration;

#if USE_WCF
using Moryx.Tools.Wcf;
using Moryx.Runtime.Wcf;
#endif

namespace Moryx.Runtime.Maintenance
{
    /// <summary>
    /// Maintenance module that hosts the plugins.
    /// </summary>
    [ServerModule(ModuleName)]
    [Description("Core module to maintain the application. It provides config, database and logging support by default. " +
                 "Additional plugins can be included as well as other extensions implementing IMaintenanceModule")]
    public class ModuleController : ServerModuleBase<ModuleConfig>
    {
        internal const string ModuleName = "Maintenance";

        #region Dependency Injection

        /// <summary>
        /// Model configurators. Injected by castle.
        /// </summary>
        public IDbContextManager DbContextManager { get; set; }

        /// <summary>
        /// runtime config manager. Injected by castle.
        /// </summary>
        public IRuntimeConfigManager RuntimeConfigManager { get; set; }

        /// <summary>
        /// Endpoint hosting
        /// </summary>
        public IEndpointHosting Hosting { get; set; }

        #endregion

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;

        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, IServerLoggerManagement loggerManagement) 
            : base(containerFactory, configManager, loggerManagement)
        {
        }

        /// <summary>
        /// Called when [initialize].
        /// </summary>
        protected override void OnInitialize()
        {
            Container.ActivateHosting(Hosting);
            Container.SetInstance(RuntimeConfigManager)
                .SetInstance(LoggerManagement);

            Container.SetInstance(DbContextManager);

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
