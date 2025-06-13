// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Moryx.Grpc;

/// <summary>
/// Configuration for the gRPC client.
/// </summary>
public class ClientConfig
{
    /// <summary>
    /// Host of the gRPC server
    /// </summary>
    /// <example>localhost</example>
    /// <example>127.0.0.1</example>
    [Description("Hostname of the gRPC server"), DefaultValue("localhost")]
    public string Host { get; set; }

    /// <summary>
    /// Hostname of the gRPC server
    /// </summary>
    /// <example>50021</example>
    [Description("Port of the gRPC server"), DefaultValue(50021)]
    public int Port { get; set; }


    /// <summary>
    /// Whether to use TLS or not
    /// </summary>
    [Description("Wheter to use TLS or not"), DefaultValue(true)]
    public bool UseTls { get; set; }

    /// <summary>
    /// Path to `.cert` file
    /// </summary>
    /// <example>https://localhost:50021</example>
    [Description("Path to `.crt` file, in case of TLS security"), DefaultValue("certificate.crt")]
    public string CertPath { get; set; }
}