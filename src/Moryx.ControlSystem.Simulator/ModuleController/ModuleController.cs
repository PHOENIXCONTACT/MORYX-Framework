// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.ControlSystem.Processes;
using Moryx.Runtime.Modules;
using System.ComponentModel;

namespace Moryx.ControlSystem.Simulator
{
    /// <summary>
    /// The main module class for the Simulation.
    /// </summary>
    [Description("Module to handle orders provided by several plugins e.g. Hydra or Web.")]
    public class ModuleController : ServerModuleBase<ModuleConfig>
    {
        internal const string ModuleName = "MachineSimulator";
        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;

        /// <summary>
        /// Resource management to access machines
        /// </summary>
        [RequiredModuleApi(IsStartDependency = true)]
        public IResourceManagement ResourceManagement { get; set; }

        /// <summary>
        /// Resource management to access machines
        /// </summary>
        [RequiredModuleApi(IsStartDependency = true)]
        public IProcessControl ProcessControl { get; set; }

        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
            : base(containerFactory, configManager, loggerFactory)
        {
        }

        #region State transition

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override void OnInitialize()
        {
            // Register required modules
            Container
                .SetInstance(ResourceManagement)
                .SetInstance(ProcessControl);
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override void OnStart()
        {
            Container.Resolve<IProcessSimulator>().Start();
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override void OnStop()
        {
            Container.Resolve<IProcessSimulator>().Stop();
        }

        #endregion
    }
}
