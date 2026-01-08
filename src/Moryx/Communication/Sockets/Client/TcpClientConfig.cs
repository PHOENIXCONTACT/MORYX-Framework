// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
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
        public override string PluginName => nameof(TcpClientConnection);

        /// <summary>
        /// Gets or sets the ip address.
        /// </summary>
        [DataMember]
        [Description("The IP-Address for this device")]
        public string IpAddress { get; set; }

        /// <summary>
        /// The port for this socket
        /// </summary>
        [DataMember, DefaultValue(5002)]
        [Description("The port for this socket")]
        public int Port { get; set; }

        /// <summary>
        /// Time to wait between attempts to open a connection in ms.
        /// </summary>
        [DataMember]
        [Description("Time to wait between attempts to open a connection in ms.")]
        [DefaultValue(500)]
        public int RetryWaitMs { get; set; }

        /// <summary>
        /// The TCP KeepAlive configuration
        /// </summary>
        [DataMember]
        [Description("The TCP KeepAlive configuration")]
        public TcpKeepAliveConfig KeepAliveConfig { get; set; }
    }
}
