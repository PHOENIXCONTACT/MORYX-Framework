// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Media.Previews;
using Moryx.Media.Server.Facades;
using Moryx.Media.Server.Previews;
using Moryx.Modules;
using Moryx.Runtime.Modules;

namespace Moryx.Media.Server
{
    /// <summary>
    /// The main controller of the media modules.
    /// </summary>
    [Description("Manages media")]
    public class ModuleController : ServerModuleBase<ModuleConfig>, IFacadeContainer<IMediaServer>
    {
        /// <summary>
        /// The module's name.
        /// </summary>
        public const string ModuleName = "MediaServer";

        /// <inheritdoc />
        public override string Name => ModuleName;

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
        protected override void OnInitialize()
        {
            Container.LoadComponents<IPreviewCreator>();
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override void OnStart()
        {
            Container.SetInstance(ConfigManager);

            StartCreators();

            var previewService = Container.Resolve<IPreviewService>();
            previewService.Start();

            var contentManager = Container.Resolve<IContentManager>();
            contentManager.Start();

            ActivateFacade(_mediaServer);
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override void OnStop()
        {
            DeactivateFacade(_mediaServer);
        }

        #endregion

        private void StartCreators()
        {
            var moduleFac = Container.Resolve<IPreviewCreatorFactory>();
            foreach (var moduleConfig in Config.PreviewCreators.Distinct())
            {
                var module = moduleFac.Create(moduleConfig);
                try
                {
                    module.Start();
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, ex, "Failed to start plugin {0}", moduleConfig.PluginName);
                    throw new Exception("Failed to start module " + moduleConfig.PluginName, ex);
                }
            }
        }

        #region FacadeContainer

        private readonly MediaServerFacade _mediaServer = new();

        IMediaServer IFacadeContainer<IMediaServer>.Facade => _mediaServer;

        #endregion
    }
}
