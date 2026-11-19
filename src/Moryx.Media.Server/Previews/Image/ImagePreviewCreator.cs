// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Media.Previews;
using Moryx.Modules;

namespace Moryx.Media.Server.Previews;

// TODO: Remove in next major

[ExpectedConfig(typeof(ImagePreviewCreatorConfig))]
[Plugin(LifeCycle.Singleton, typeof(IPreviewCreator), Name = nameof(ImagePreviewCreator))]
internal class ImagePreviewCreator : PreviewCreatorBase<ImagePreviewCreatorConfig>
{
    private const string SupportErrorMessage = "Image preview creation is not supported in this build. Please use the FFmpegPreviewCreator instead.";

    /// <inheritdoc />
    public override bool CanCreate(string mimeType)
    {
        Logger.LogWarning(SupportErrorMessage);
        return false;
    }

    /// <inheritdoc />
    public override Task<PreviewJobResult> CreatePreviewAsync(PreviewJob job, CancellationToken cancellationToken)
    {
        throw new NotSupportedException(SupportErrorMessage);
    }
}
