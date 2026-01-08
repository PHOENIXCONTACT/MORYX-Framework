// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moryx.Communication.Sockets;

/// <summary>
/// Configuration for the TCP Keep-Alive.
/// TCP Keep-Alive is a mechanism that allows a TCP connection to detect whether the remote peer is still reachable, even when no application data is being exchanged.
/// </summary>
[DataContract]
public class TcpKeepAliveConfig
{
    /// <summary>
    /// Use keep-alive on this socket. OS defaults are used when override is inactive.
    /// </summary>
    [DataMember]
    [Description("Use keep-alive on this socket. OS defaults are used when override is inactive.")]
    public bool UseKeepAlive { get; set; }

    /// <summary>
    /// Override operating system defaults with this configuration.
    /// </summary>
    [DataMember]
    [Description("Override operating system defaults with this configuation.")]
    public bool OverrideOsDefaults { get; set; }

    /// <summary>
    /// The number of seconds a TCP connection will wait for a keepalive response before sending another keepalive probe.
    /// </summary>
    [DataMember, DefaultValue(75)]
    [Description("The number of seconds a TCP connection will wait for a keepalive response before sending another keepalive probe.")]
    public int KeepAliveInterval { get; set; }

    /// <summary>
    /// The number of seconds a TCP connection will remain alive/idle before keepalive probes are sent to the remote.
    /// </summary>
    [DataMember, DefaultValue(7200)]
    [Description("The number of seconds a TCP connection will remain alive/idle before keepalive probes are sent to the remote.")]
    public int KeepAliveTime { get; set; }

    /// <summary>
    /// The number of TCP keep alive probes that will be sent before the connection is terminated.
    /// </summary>
    [DataMember, DefaultValue(9)]
    [Description("The number of TCP keep alive probes that will be sent before the connection is terminated.")]
    public int KeepAliveRetryCount { get; set; }
}
