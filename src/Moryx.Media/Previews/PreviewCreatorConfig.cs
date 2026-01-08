// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Modules;

namespace Moryx.Media.Previews
{
    /// <summary>
    /// Base class for <see cref="IPreviewCreator"/> plugins
    /// </summary>
    [DataContract]
    public abstract class PreviewCreatorConfig : IPluginConfig
    {
        /// <inheritdoc />
        public abstract string PluginName { get; }
    }
}

