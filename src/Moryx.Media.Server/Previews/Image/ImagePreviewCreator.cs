// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Media.Previews;
using Moryx.Modules;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Moryx.Media.Server.Previews
{
    [ExpectedConfig(typeof(ImagePreviewCreatorConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IPreviewCreator), Name = nameof(ImagePreviewCreator))]
    internal class ImagePreviewCreator : PreviewCreatorBase<ImagePreviewCreatorConfig>
    {
        private static readonly string[] SupportedMimeTypes;

        static ImagePreviewCreator()
        {
            SupportedMimeTypes = SixLabors.ImageSharp.Configuration.Default.ImageFormatsManager.ImageFormats.SelectMany(f => f.MimeTypes).ToArray();
        }

        /// <inheritdoc />
        public override bool CanCreate(string mimeType) =>
            SupportedMimeTypes.Contains(mimeType);

        /// <inheritdoc />
        public override async Task<PreviewJobResult> CreatePreviewAsync(PreviewJob job, CancellationToken cancellationToken)
        {
            var result = new PreviewJobResult();
            try
            {
                using var image = await Image.LoadAsync(job.SourcePath, cancellationToken);

                int width = 0, height = 0;
                if (image.Width > image.Height)
                    width = Config.LongestEdge;
                else if (image.Height > image.Width)
                    height = Config.LongestEdge;
                else
                {
                    width = Config.LongestEdge;
                    height = Config.LongestEdge;
                }

                image.Mutate(x => x
                    .Resize(width, height));

                var targetStream = new MemoryStream();
                var encoder = new JpegEncoder { Quality = Config.Quality };
                await image.SaveAsJpegAsync(targetStream, encoder, cancellationToken);

                result.Preview = targetStream;
                result.MimeType = "image/jpeg";
                result.Size = targetStream.Length;
                result.PreviewState = PreviewState.Created;

            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e, $"Cannot create preview from {job.SourcePath}");
                result.PreviewState = PreviewState.Failed;
            }

            return result;
        }
    }
}
