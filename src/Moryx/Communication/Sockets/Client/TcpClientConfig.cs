// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moryx.Communication.Sockets
{
    /// <summary>
    /// Config for TcpClients
    /// </summary>
    [DataContract]
    public class TcpClientConfig : BinaryConnectionConfig
    {
        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        /// <value>
        /// The name of the plugin.
        /// </value>
        public override string PluginName => nameof(TcpClientConnection);

        /// <summary>
        /// Gets or sets the ip address.
        /// </summary>
        /// <value>
        /// The ip address.
        /// </value>
        [DataMember]
        [Description("The IP-Address for this device")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        [DataMember]
        [Description("The TCP-Port for this Device")]
        [DefaultValue(5002)]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the connect retry wait time in ms.
        /// </summary>
        /// <value>
        /// The connect retry wait ms.
        /// </value>
        [DataMember]
        [Description("Time to wait between attempts to open a connection in ms.")]
        [DefaultValue(500)]
        public int RetryWaitMs { get; set; }

        /// <summary>
        /// Time in milliseconds to check if connection is still open. Disable with -1
        /// </summary>
        [DataMember, DefaultValue(5000)]
        [Description("Time in milliseconds to check if connection is still open. Disable with -1")]
        public int MonitoringIntervalMs { get; set; }

        /// <summary>
        /// Timeout for a monitoring call
        /// </summary>
        [DataMember, DefaultValue(500)]
        [Description("Timeout for a monitoring call")]
        public int MonitoringTimeoutMs { get; set; }
    }
}
