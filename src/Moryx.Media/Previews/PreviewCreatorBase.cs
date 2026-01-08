// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;

namespace Moryx.Media.Previews;

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
    public virtual Task InitializeAsync(PreviewCreatorConfig config, CancellationToken cancellationToken = default)
    {
        Config = (TConf)config;
        Logger = Logger.GetChild(config.PluginName, GetType());

        return Task.CompletedTask;
    }

    /// <summary>
    /// Starts the preview creator
    /// </summary>
    public virtual Task StartAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<PreviewJobResult> CreatePreviewAsync(PreviewJob job, CancellationToken cancellationToken);
}