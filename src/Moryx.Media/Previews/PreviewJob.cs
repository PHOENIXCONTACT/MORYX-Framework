// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Media.Previews
{
    /// <summary>
    /// Preview creator job settings
    /// </summary>
    public class PreviewJob
    {
        /// <summary>
        /// Target descriptor Id to which the previews will belong to
        /// </summary>
        public ContentDescriptor ContentDescriptor { get; set; }

        /// <summary>
        /// Target variant to which the previews will be belong to
        /// </summary>
        public VariantDescriptor Variant { get; set; }

        /// <summary>
        /// Path to the preview image
        /// </summary>
        public string SourcePath { get; set; }
    }
}

