// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.Model;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kestrel.Maintenance
{
    /// <summary>
    /// Maintenance module that hosts the plugins.
    /// </summary>
    [ServerModule(ModuleName)]
    [Description("Core module to maintain the application. It provides config, database and logging support by default. " +
                 "Additional plugins can be included as well as other extensions implementing IMaintenanceModule")]
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
        public IDbContextManager DbContextManager { get; set; }

        /// <summary>
        /// runtime config manager. Injected by castle.
        /// </summary>
        public IRuntimeConfigManager RuntimeConfigManager { get; set; }

        /// <summary>
        /// Kestrel support
        /// </summary>
        public IKestrelHttpHostFactory KestrelHttpHostFactory { get; set; }

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
            Container.RegisterToKestrel(KestrelHttpHostFactory)
                .SetInstance(_moduleManager)
                .SetInstance(RuntimeConfigManager)
                .SetInstance((IServerLoggerManagement) LoggerManagement)
                .SetInstance(DbContextManager)
                .SetInstance(Config.DatabaseConfig)
                .SetInstance(Config.LoggingMaintenanceConfig);
        }

        /// <summary>
        /// Called when [start].
        /// </summary>
        /// <exception cref="System.Exception">Failed to start module  + moduleConfig.PluginName</exception>
        protected override void OnStart()
        {
            var loggingPlugin = Container.Resolve<ILoggingAppender>();

            loggingPlugin.Initialize(Config.LoggingMaintenanceConfig);
            loggingPlugin.Start();
        }

        /// <summary>
        /// Called when [stop].
        /// </summary>
        protected override void OnStop()
        {
        }

        /// <inheritdoc />
        protected override void OnDestruct()
        {
            Container.UnregisterFromKestrel(KestrelHttpHostFactory);
        }
    }
}
