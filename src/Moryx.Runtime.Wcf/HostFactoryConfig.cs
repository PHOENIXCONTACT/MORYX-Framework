// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Runtime.Wcf
{
    /// <summary>
    /// Configuration for the host factory.
    /// </summary>
    [DataContract]
    [Obsolete("The version service is required as soon as at least one WCF endpoint is availabe, so the flag is ignored now!")]
    public class HostFactoryConfig : ConfigBase
    {
        /// <summary>
        /// Configuration to enable/disable the version service.
        /// </summary>
        [DataMember]
        public bool VersionServiceDisabled { get; set; }
    }
}
