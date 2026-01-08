// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Media.Previews;

namespace Moryx.Media.Server.Previews
{
    [DataContract]
    internal class ImagePreviewCreatorConfig : PreviewCreatorConfig
    {
        public override string PluginName => nameof(ImagePreviewCreator);

        [DataMember, DefaultValue(250)]
        [Description("Defines how long the longest edge of the resulting preview should be")]
        public int LongestEdge { get; set; }

        [DataMember, DefaultValue(80)]
        [Description("Image quality between 0 - 100")]
        public int Quality { get; set; }
    }
}

