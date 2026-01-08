// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.VisualInstructions.Controller
{
    /// <summary>
    /// Module controller of the ProcessEngine
    /// </summary>
    [Description("Server module to manage and publish visual instructions")]
    public class ModuleController : ServerModuleBase<ModuleConfig>, IFacadeContainer<IVisualInstructions>
    {
        /// <summary>
        /// The module's name.
        /// </summary>
        public const string ModuleName = "VisualInstructions";

        /// <inheritdoc />
        public override string Name => ModuleName;

        /// <summary>
        /// Create new module instance
        /// </summary>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory) : base(containerFactory, configManager, loggerFactory)
        {
        }

        #region Generated imports

        /// <summary>
        /// Resource management facade that allows communication with different hardware within
        /// the machine
        /// </summary>
        [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
        public IResourceManagement ResourceManagement { get; set; }

        #endregion

        #region State transition

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            // Register all imported components
            Container.SetInstance(ResourceManagement);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override Task OnStartAsync(CancellationToken cancellationToken)
        {
            Container.Resolve<IVisualInstructionsController>().Start();

            // Activate facade
            ActivateFacade(_visualInstructionsFacade);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override Task OnStopAsync(CancellationToken cancellationToken)
        {
            // Deactivate facade
            DeactivateFacade(_visualInstructionsFacade);

            Container.Resolve<IVisualInstructionsController>().Stop();
            return Task.CompletedTask;
        }

        #endregion

        #region FacadeContainer

        private readonly VisualInstructionsFacade _visualInstructionsFacade = new();

        IVisualInstructions IFacadeContainer<IVisualInstructions>.Facade => _visualInstructionsFacade;

        #endregion
    }
}
