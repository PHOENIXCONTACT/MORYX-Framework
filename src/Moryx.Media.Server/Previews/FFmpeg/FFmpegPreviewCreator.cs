// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Media.Previews;
using Moryx.Modules;

namespace Moryx.Media.Server.Previews
{
    [ExpectedConfig(typeof(FFmpegPreviewCreatorConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IPreviewCreator), Name = nameof(FFmpegPreviewCreator))]
    internal class FFmpegPreviewCreator : PreviewCreatorBase<FFmpegPreviewCreatorConfig>
    {
        private string _ffmpeg;
        private static readonly string[] SupportedMimeTypes =
        {
            "video/mp4", "video/x-m4v", "video/mpeg", "video/ogg", "video/x-matroska", "video/webm", "video/x-msvideo"
        };

        /// <inheritdoc />
        public override void Initialize(PreviewCreatorConfig config)
        {
            base.Initialize(config);

            var ffmpeg = Path.Combine(Config.FFmpegPath, "ffmpeg.exe");
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
            SupportedMimeTypes.Contains(mimeType) && !string.IsNullOrEmpty(_ffmpeg);

        /// <inheritdoc />
        public override PreviewJobResult CreatePreview(PreviewJob job)
        {
            var result = new PreviewJobResult();

            // ReSharper disable once AssignNullToNotNullAttribute
            var tempPreview = Path.Combine(Path.GetTempPath(), job.Variant.FileHash);
            var args = $"-itsoffset -1 -i \"{job.SourcePath}\" -vcodec mjpeg -vframes 1 -an -f rawvideo \"{tempPreview}\"";

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
            var exited = process.WaitForExit(5000);
            if (!exited)
                process.Kill();

            if (!File.Exists(tempPreview))
            {
                Logger.Log(LogLevel.Error, "The preview was not created for inexplicable reasons");
                result.PreviewState = PreviewState.Failed;
                return result;
            }

            var ms = new MemoryStream(File.ReadAllBytes(tempPreview));
            File.Delete(tempPreview);

            result.Preview = ms;
            result.MimeType = "image/jpeg";
            result.Size = ms.Length;
            result.PreviewState = PreviewState.Created;

            return result;
        }
    }
}
