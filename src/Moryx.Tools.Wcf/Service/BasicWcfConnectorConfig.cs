// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Basic WCF service configuration
    /// </summary>
    [DataContract]
    public abstract class BasicWcfConnectorConfig : UpdatableEntry, IWcfServiceConfig
    {
        /// <summary>
        /// The plugin name
        /// </summary>
        public virtual string PluginName { get; protected set; }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        protected BasicWcfConnectorConfig(string endpoint)
        {
            ConnectorHost = new HostConfig
            {
                BindingType = ServiceBindingType.NetTcp,
                Endpoint = endpoint,
                MetadataEnabled = true,
            };
        }

        /// <summary>
        /// Gets or sets the connector host.
        /// </summary>
        /// <value> The connector host. </value>
        [DataMember]
        [Description("Host config")]
        public HostConfig ConnectorHost { get; set; }
    }
}
