// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;

namespace Moryx.Media.Previews
{
    /// <summary>
    /// Base class for preview creator plugins
    /// </summary>
    /// <typeparam name="TConf"></typeparam>
    public abstract class PreviewCreatorBase<TConf> : IPreviewCreator where TConf : PreviewCreatorConfig
    {
        /// <summary>
        /// Configuration of this creator plugin
        /// </summary>
        protected TConf Config { get; private set; }

        /// <summary>
        /// Logger for this creator
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Initializes the preview creator
        /// </summary>
        /// <param name="config"></param>
        public virtual void Initialize(PreviewCreatorConfig config)
        {
            Config = (TConf) config;
            Logger = Logger.GetChild(config.PluginName, GetType());
        }

        /// <summary>
        /// Starts the preview creator
        /// </summary>
        public virtual void Start()
        {
        }

        /// <inheritdoc />
        public virtual void Stop()
        {
        }

        /// <summary>
        /// Returns true if the given mime type is supported by this preview creator
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns></returns>
        public abstract bool CanCreate(string mimeType);

        /// <summary>
        /// Creates one or more previews
        /// </summary>
        /// <param name="job">Settings and information about the source file</param>
        /// <returns></returns>
        public abstract PreviewJobResult CreatePreview(PreviewJob job);
    }
}

