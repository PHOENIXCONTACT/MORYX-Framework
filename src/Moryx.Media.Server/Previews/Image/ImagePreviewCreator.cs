// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#if NETFRAMEWORK
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Media.Previews;
using Moryx.Modules;

namespace Moryx.Media.Server.Previews
{
    [ExpectedConfig(typeof(ImagePreviewCreatorConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IPreviewCreator), Name = nameof(ImagePreviewCreator))]
    internal class ImagePreviewCreator : PreviewCreatorBase<ImagePreviewCreatorConfig>
    {
        private static readonly string[] SupportedMimeTypes;
        private static readonly ImageCodecInfo JpegEncoder = GetEncoder(ImageFormat.Jpeg);

        static ImagePreviewCreator()
        {
            SupportedMimeTypes = ImageCodecInfo.GetImageDecoders().Select(e => e.MimeType).ToArray();
        }

        /// <inheritdoc />
        public override bool CanCreate(string mimeType) =>
            SupportedMimeTypes.Contains(mimeType);

        /// <inheritdoc />
        public override PreviewJobResult CreatePreview(PreviewJob job)
        {
            var result = new PreviewJobResult();
            try
            {
                using var inputFileStream = new FileStream(job.SourcePath, FileMode.Open);
                var targetStream = new MemoryStream();
                var image = Image.FromStream(inputFileStream);

                var destRect = CalculatePreviewSize(image.Size, Config.LongestEdge);
                var destImage = new Bitmap(destRect.Width, destRect.Height);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using var graphics = Graphics.FromImage(destImage);
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }

                var qualityEncoder = Encoder.Quality;
                var encoderParameters = new EncoderParameters(1)
                {
                    Param = { [0] = new EncoderParameter(qualityEncoder, Config.Quality) }
                };

                destImage.Save(targetStream, JpegEncoder, encoderParameters);

                result.Preview = targetStream;
                result.MimeType = JpegEncoder.MimeType;
                result.Size = targetStream.Length;
                result.PreviewState = PreviewState.Created;
            }
            catch (Exception e)
            {
                Logger.LogException(LogLevel.Error, e, $"Cannot create preview from {job.SourcePath}");
                result.PreviewState = PreviewState.Failed;
            }

            return result;
        }

        private static Rectangle CalculatePreviewSize(Size originalSize, int longestEdgeLength)
        {
            var previewSize = new Rectangle();
            if (originalSize.Width > originalSize.Height)
            {
                previewSize.Width = longestEdgeLength;
                previewSize.Height = (int)((double)originalSize.Height / originalSize.Width * longestEdgeLength);
            }
            else if (originalSize.Height > originalSize.Width)
            {
                previewSize.Width = (int)((double)originalSize.Width / originalSize.Height * longestEdgeLength);
                previewSize.Height = longestEdgeLength;
            }
            else
            {
                previewSize.Width = longestEdgeLength;
                previewSize.Height = longestEdgeLength;
            }

            return previewSize;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            var codec = codecs.FirstOrDefault(c => c.FormatID == format.Guid);
            if (codec == null)
                throw new NotSupportedException($"Format unknown: ${format}");

            return codec;
        }
    }

}
#else
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
        public override PreviewJobResult CreatePreview(PreviewJob job)
        {
            var result = new PreviewJobResult();
            try
            {
                using var image = Image.Load(job.SourcePath);

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
                var encoder = new JpegEncoder {Quality = Config.Quality};
                image.SaveAsJpeg(targetStream, encoder);

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
#endif
