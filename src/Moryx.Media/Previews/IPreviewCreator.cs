// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Media.Previews
{
    /// <summary>
    /// Interface that a preview creator has to implement
    /// </summary>
    public interface IPreviewCreator : IAsyncConfiguredPlugin<PreviewCreatorConfig>
    {
        /// <summary>
        /// Indicates whether the given mime type is supported by the implementation
        /// </summary>
        /// <param name="mimeType">Mime type to be checked</param>
        /// <returns>True if the creator supports the mime type</returns>
        bool CanCreate(string mimeType);

        /// <summary>
        /// Creates one previews
        /// </summary>
        /// <param name="job">A job that describes the preview creation</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>True if preview creation was successful otherwise false</returns>
        Task<PreviewJobResult> CreatePreviewAsync(PreviewJob job, CancellationToken cancellationToken);
    }
}

