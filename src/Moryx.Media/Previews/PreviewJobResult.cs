// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Media.Previews;

/// <summary>
/// Result of a preview creation operation
/// </summary>
public class PreviewJobResult : IDisposable
{
    /// <summary>
    /// State of the previews
    /// </summary>
    public PreviewState PreviewState { get; set; }

    /// <summary>
    /// Produced previews
    /// </summary>
    public Stream Preview { get; set; }

    /// <summary>
    /// Mime type of the preview
    /// </summary>
    public string MimeType { get; set; }

    /// <summary>
    /// Size of the preview
    /// </summary>
    public long Size { get; set; }

    /// <inheritdoc />
    public void Dispose()
    {
        Preview?.Dispose();
    }
}