// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Media.Previews;
using Moryx.Media.Server.Facades;
using Moryx.Media.Server.Previews;
using Moryx.Runtime.Modules;

namespace Moryx.Media.Server
{
    /// <summary>
    /// The main controller of the media modules.
    /// </summary>
    [Description("Manages media")]
    public class ModuleController : ServerModuleBase<ModuleConfig>, IFacadeContainer<IMediaServer>
    {
        /// <inheritdoc />
        public override string Name => "MediaServer";

        /// <summary>
        /// Create new module instance
        /// </summary>
        /// <param name="containerFactory"></param>
        /// <param name="configManager"></param>
        /// <param name="loggerFactory"></param>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory) : base(containerFactory, configManager, loggerFactory)
        {
        }

        #region State transition

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Container.LoadComponents<IPreviewCreator>();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override async Task OnStartAsync(CancellationToken cancellationToken)
        {
            Container.SetInstance(ConfigManager);

            await StartCreators(cancellationToken);

            var previewService = Container.Resolve<IPreviewService>();
            previewService.Start();

            var contentManager = Container.Resolve<IContentManager>();
            contentManager.Start();

            ActivateFacade(_mediaServer);
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override Task OnStopAsync(CancellationToken cancellationToken)
        {
            DeactivateFacade(_mediaServer);

            // TODO stop preview creators

            return Task.CompletedTask;
        }

        #endregion

        private async Task StartCreators(CancellationToken cancellationToken)
        {
            var previewCreatorFactory = Container.Resolve<IPreviewCreatorFactory>();
            foreach (var creatorConfig in Config.PreviewCreators.Distinct())
            {
                var module = await previewCreatorFactory.Create(creatorConfig, cancellationToken);
                try
                {
                    await module.StartAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, ex, "Failed to start preview creator {previewCreator}", creatorConfig.PluginName);
                    throw;
                }
            }
        }

        #region FacadeContainer

        private readonly MediaServerFacade _mediaServer = new();

        IMediaServer IFacadeContainer<IMediaServer>.Facade => _mediaServer;

        #endregion
    }
}
