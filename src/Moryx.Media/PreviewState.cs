// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Media
{
    /// <summary>
    /// State of the preview
    /// </summary>
    public enum PreviewState
    {
        /// <summary>
        /// Preview creation wa triggered
        /// </summary>
        Creating,

        /// <summary>
        /// At least one preview was created
        /// </summary>
        Created,

        /// <summary>
        /// Preview creation failed
        /// </summary>
        Failed,

        /// <summary>
        /// Preview for the given variant is not supported
        /// </summary>
        Unsupported,

        /// <summary>
        /// Preview was removed and is not available anymore
        /// </summary>
        Removed
    }
}
