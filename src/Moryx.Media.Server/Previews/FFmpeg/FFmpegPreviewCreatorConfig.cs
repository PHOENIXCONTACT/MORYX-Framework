// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Media.Previews;

namespace Moryx.Media.Server.Previews;

[DataContract]
internal class FFmpegPreviewCreatorConfig : PreviewCreatorConfig
{
    public override string PluginName => nameof(FFmpegPreviewCreator);

    [DataMember]
    [Description("Path where the FFmpeg executable can be found.")]
    public string FFmpegPath { get; set; }

    [DataMember, DefaultValue(5000)]
    [Description("Timeout for FFmpeg to create a preview in milliseconds")]
    public int Timeout { get; set; }

    [DataMember, DefaultValue(250)]
    [Description("Defines how long the longest edge of the resulting image preview should be")]
    public int LongestEdge { get; set; }

    [DataMember, DefaultValue(80)]
    [Description("Image quality between 0 - 100")]
    public int Quality { get; set; }
}
