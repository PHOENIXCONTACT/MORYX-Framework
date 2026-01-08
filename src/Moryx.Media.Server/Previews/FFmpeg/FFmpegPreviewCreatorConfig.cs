// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Media.Previews;

namespace Moryx.Media.Server.Previews
{
    [DataContract]
    internal class FFmpegPreviewCreatorConfig : PreviewCreatorConfig
    {
        public override string PluginName => nameof(FFmpegPreviewCreator);

        [DataMember]
        [Description("Path where the FFmpeg.exe can be found.")]
        public string FFmpegPath { get; set; }

        [DataMember, DefaultValue(5000)]
        [Description("Timeout for FFmpeg to create a preview from video")]
        public int Timeout { get; set; }
    }
}
