// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Media.Previews;
using Moryx.Media.Server.Previews;
using Moryx.Serialization;

namespace Moryx.Media.Server
{
    /// <summary>
    /// Module configuration of the MediaServer <see cref="ModuleController"/>
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// Storage path for media items
        /// </summary>
        [DataMember]
        [Description("Storage path of the media")]
        public string StoragePath { get; set; } = "Backups\\MediaServer";

        /// <summary>
        /// Maximum file size for media items in MB
        /// </summary>
        [DataMember]
        [Description("Maximum file size in MB")]
        public int MaxFileSizeInMb { get; set; } = 10;

        /// <summary>
        /// List of allowed file types
        /// </summary>
        [DataMember]
        [Description("Allowed file types defined by extension")]
        public string[] SupportedFileTypes { get; set; } = { ".png", ".jpeg", ".jpg", ".gif", ".pdf", ".txt", ".csv" };

        /// <summary>
        /// Preview creator plugin configurations
        /// </summary>
        [DataMember]
        [PluginConfigs(typeof(IPreviewCreator), false)]
        [Description("List of configured preview creator plugins.")]
        public PreviewCreatorConfig[] PreviewCreators { get; set; }

        /// <inheritdoc />
        public override void Initialize()
        {
            PreviewCreators = new PreviewCreatorConfig[]
            {
                new ImagePreviewCreatorConfig()
            };
        }
    }
}
