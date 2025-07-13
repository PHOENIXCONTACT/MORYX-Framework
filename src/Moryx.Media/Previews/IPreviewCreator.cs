// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Media.Previews
{
    /// <summary>
    /// Interface that a preview creator has to implement
    /// </summary>
    public interface IPreviewCreator : IConfiguredPlugin<PreviewCreatorConfig>
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
        /// <returns>True if preview creation was successful otherwise false</returns>
        PreviewJobResult CreatePreview(PreviewJob job);
    }
}

