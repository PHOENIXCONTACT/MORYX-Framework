// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moryx.Communication.Sockets
{
    /// <summary>
    /// Config for <see cref="TcpListenerConnection"/>
    /// </summary>
    [DataContract]
    public class TcpListenerConfig : BinaryConnectionConfig
    {
        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        public override string PluginName => nameof(TcpListenerConnection);

        /// <summary>
        /// Gets or sets the ip address. Usually this is IPAddress.Any which (for IPv4) is "0.0.0.0"
        /// </summary>
        [DataMember, DefaultValue("0.0.0.0")]
        [Description("The IP-Address for this socket")]
        public string IpAddress { get; set; }

        /// <summary>
        /// The port for this socket
        /// </summary>
        [DataMember, DefaultValue(5002)]
        [Description("The port for this socket")]
        public int Port { get; set; }

        /// <summary>
        /// Validate incoming messages before assigning the connection, even if this is the only listener.
        /// </summary>
        [DataMember]
        [Description("Validate incoming messages before assigning the connection, even if this is the only listener.")]
        public bool ValidateBeforeAssignment { get; set; }

        /// <summary>
        /// The TCP KeepAlive configuration
        /// </summary>
        [DataMember]
        [Description("The TCP KeepAlive configuration")]
        public TcpKeepAliveConfig KeepAliveConfig { get; set; }
    }
}
