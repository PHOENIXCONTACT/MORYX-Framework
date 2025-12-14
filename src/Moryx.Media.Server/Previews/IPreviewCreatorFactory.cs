// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Media.Previews;

namespace Moryx.Media.Server.Previews
{
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface IPreviewCreatorFactory
    {
        /// <summary>
        /// Create the maintenance module with this name
        /// </summary>
        Task<IPreviewCreator> Create(PreviewCreatorConfig config, CancellationToken cancellationToken);

        /// <summary>
        /// Destroy a module instance
        /// </summary>
        void Destroy(IPreviewCreator instance);
    }
}

