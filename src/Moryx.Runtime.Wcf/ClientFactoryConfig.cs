// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    /// <summary>
    /// Configuration for the wcf client factory.
    /// </summary>
    [DataContract]
    public class ClientFactoryConfig : ConfigBase, IWcfClientFactoryConfig
    {
        /// <summary>
        /// Config entry for the host address.
        /// </summary>
        [DataMember]
        [DefaultValue("localhost")]
        public string Host { get; set; }

        /// <summary>
        /// Config entry for the port.
        /// </summary>
        [DataMember]
        [DefaultValue(80)]
        public int Port { get; set; }

        /// <summary>
        /// Config entry for the Wcf unique clientId.
        /// </summary>
        [DataMember]
        [DefaultValue("localhost")]
        public string ClientId { get; set; }
    }
}
