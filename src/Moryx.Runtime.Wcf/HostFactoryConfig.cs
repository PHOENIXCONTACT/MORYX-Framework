// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Runtime.Wcf
{
    /// <summary>
    /// Configuration for the host factory.
    /// </summary>
    [DataContract]
    public class HostFactoryConfig : ConfigBase
    {
        /// <summary>
        /// Configuration to enable/disable the version service.
        /// </summary>
        [DataMember]
        public bool VersionServiceDisabled { get; set; }
    }
}
