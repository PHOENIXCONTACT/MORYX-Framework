// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moryx.Communication.Sockets
{
    /// <summary>
    /// Config for TCPListeners
    /// </summary>
    [DataContract]
    public class TcpListenerConfig : BinaryConnectionConfig
    {
        /// 
        public override string PluginName => nameof(TcpListenerConnection);

        /// <summary>
        /// Gets or sets the ip adress. Usually this is IPAddress.Any which (for IPv4) is "0.0.0.0"
        /// </summary>
        [DataMember, DefaultValue("0.0.0.0")]
        [Description("The IPAdress for this device")]
        public string IpAdress { get; set; }

        /// <summary>
        /// TCP Port of the connection
        /// </summary>
        [DataMember, DefaultValue(5002)]
        [Description("The TCP-Port for this Device")]
        public int Port { get; set; }

        /// <summary>
        /// Flag if incoming connections must be validated before assigning them, even if this is the only listener
        /// </summary>
        [DataMember]
        [Description("Validate incoming messages before assigning the connection, even if this is the only listener.")]
        public bool ValidateBeforeAssignment { get; set; }

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
