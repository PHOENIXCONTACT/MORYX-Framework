// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Media.Previews;
using Moryx.Modules;

namespace Moryx.Media.Server.Previews;

[ExpectedConfig(typeof(FFmpegPreviewCreatorConfig))]
[Plugin(LifeCycle.Singleton, typeof(IPreviewCreator), Name = nameof(FFmpegPreviewCreator))]
internal class FFmpegPreviewCreator : PreviewCreatorBase<FFmpegPreviewCreatorConfig>
{
    private string _ffmpeg;

    private static readonly string[] SupportedVideoMimeTypes =
    [
        "video/mp4", "video/quicktime", "video/x-m4v", "video/mpeg", "video/ogg", "video/x-matroska", "video/webm", "video/x-msvideo"
    ];

    private static readonly string[] SupportedImageMimeTypes =
    [
        "image/jpeg", "image/png", "image/tiff", "image/bmp"
    ];

    /// <inheritdoc />
    public override async Task InitializeAsync(PreviewCreatorConfig config, CancellationToken cancellationToken = default)
    {
        await base.InitializeAsync(config, cancellationToken);

        if (string.IsNullOrEmpty(Config.FFmpegPath))
        {
            Logger.Log(LogLevel.Warning, "FFmpeg path is not configured. Preview creator is holding back.");
            _ffmpeg = string.Empty;
            return;
        }

        var ffmpeg = Path.Combine(Config.FFmpegPath, "ffmpeg");
        if (File.Exists(ffmpeg))
        {
            _ffmpeg = ffmpeg;
        }
        else
        {
            Logger.Log(LogLevel.Warning, "FFmpeg was not found. Preview creator is holding back: {0}", ffmpeg);
            _ffmpeg = string.Empty;
        }
    }

    /// <inheritdoc />
    public override bool CanCreate(string mimeType) =>
        (SupportedVideoMimeTypes.Contains(mimeType) || SupportedImageMimeTypes.Contains(mimeType))
        && !string.IsNullOrEmpty(_ffmpeg);

    /// <inheritdoc />
    public override Task<PreviewJobResult> CreatePreviewAsync(PreviewJob job, CancellationToken cancellationToken)
    {
        var isImage = SupportedImageMimeTypes.Contains(job.Variant.MimeType);
        return isImage ? CreateImagePreviewAsync(job) : CreateVideoPreviewAsync(job);
    }

    private Task<PreviewJobResult> CreateImagePreviewAsync(PreviewJob job)
    {
        var result = new PreviewJobResult();

        var tempPreview = Path.Combine(Path.GetTempPath(), job.Variant.FileHash + ".jpg");
        var longestEdge = Config.LongestEdge;
        var quality = Config.Quality;
        var args = $"-i \"{job.SourcePath}\" -vf \"scale='if(gte(iw,ih),{longestEdge},-1)':'if(gt(ih,iw),{longestEdge},-1)'\" -q:v {QualityToFFmpegScale(quality)} -y \"{tempPreview}\"";

        var success = RunFFmpeg(args);
        if (!success || !File.Exists(tempPreview))
        {
            Logger.Log(LogLevel.Error, "Cannot create image preview from {0}", job.SourcePath);
            result.PreviewState = PreviewState.Failed;
            return Task.FromResult(result);
        }

        var ms = new MemoryStream(File.ReadAllBytes(tempPreview));
        File.Delete(tempPreview);

        result.Preview = ms;
        result.MimeType = "image/jpeg";
        result.Size = ms.Length;
        result.PreviewState = PreviewState.Created;

        return Task.FromResult(result);
    }

    private Task<PreviewJobResult> CreateVideoPreviewAsync(PreviewJob job)
    {
        var result = new PreviewJobResult();

        // ReSharper disable once AssignNullToNotNullAttribute
        var tempPreview = Path.Combine(Path.GetTempPath(), job.Variant.FileHash + ".jpg");
        var longestEdge = Config.LongestEdge;
        var quality = Config.Quality;
        var args = $"-itsoffset -1 -i \"{job.SourcePath}\" -vframes 1 -vf \"scale='if(gte(iw,ih),{longestEdge},-1)':'if(gt(ih,iw),{longestEdge},-1)'\" -q:v {QualityToFFmpegScale(quality)} -y \"{tempPreview}\"";

        var success = RunFFmpeg(args);
        if (!success || !File.Exists(tempPreview))
        {
            Logger.Log(LogLevel.Error, "The preview was not created for inexplicable reasons");
            result.PreviewState = PreviewState.Failed;
            return Task.FromResult(result);
        }

        var ms = new MemoryStream(File.ReadAllBytes(tempPreview));
        File.Delete(tempPreview);

        result.Preview = ms;
        result.MimeType = "image/jpeg";
        result.Size = ms.Length;
        result.PreviewState = PreviewState.Created;

        return Task.FromResult(result);
    }

    private bool RunFFmpeg(string args)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = _ffmpeg,
                Arguments = args
            }
        };

        process.Start();
        var exited = process.WaitForExit(Config.Timeout);
        if (!exited)
        {
            process.Kill();
            return false;
        }

        return process.ExitCode == 0;
    }

    /// <summary>
    /// Converts a quality value (0-100) to the FFmpeg JPEG quality scale (1-31, where 1 is best)
    /// </summary>
    private static int QualityToFFmpegScale(int quality)
    {
        var clamped = Math.Clamp(quality, 0, 100);
        return 31 - (int)(clamped / 100.0 * 30);
    }
}
