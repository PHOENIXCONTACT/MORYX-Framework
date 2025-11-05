// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.Analytics.Server.ModuleController
{
    /// <summary>
    /// Module controller of the media module.
    /// </summary>
    [Description("Modul to analyze data and visualize in Dashboards")]
    public class ModuleController : ServerModuleBase<ModuleConfig>
    {
        /// <summary>
        /// The module's name.
        /// </summary>
        public const string ModuleName = "Analytics";

        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory) : base(containerFactory, configManager, loggerFactory)
        {
        }

        /// <inheritdoc />
        public override string Name => ModuleName;

        #region State transition

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override void OnInitialize()
        {
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override void OnStart()
        {
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override void OnStop()
        {
        }

        #endregion
    }
}
