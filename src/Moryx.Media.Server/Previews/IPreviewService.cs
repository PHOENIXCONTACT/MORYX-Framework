// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Media.Previews;
using Moryx.Modules;

namespace Moryx.Media.Server.Previews
{
    internal delegate void PreviewCreationResultHandler(PreviewJob job, PreviewJobResult result);

    internal interface IPreviewService : IPlugin
    {
        void QueuePreviewJob(PreviewJob job);

        event PreviewCreationResultHandler PreviewJobCompleted;
    }
}

