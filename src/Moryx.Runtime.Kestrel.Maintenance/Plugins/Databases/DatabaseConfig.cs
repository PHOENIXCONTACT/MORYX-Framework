// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.Runtime.Kestrel.Maintenance
{
    /// <summary>
    /// Configuration for the database maintenance plugin.
    /// </summary>
    [DataContract]
    public class DatabaseConfig
    {
        /// <summary>
        /// Folder where the setup data is stored.
        /// </summary>
        [DataMember]
        [RelativeDirectories]
        [DefaultValue(@".\Backups\")]
        public string SetupDataDir { get; set; }
    }
}
