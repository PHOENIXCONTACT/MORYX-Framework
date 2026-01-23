// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.ProcessData.InfluxDbListener;

/// <summary>
/// Extended config for influx 2
/// </summary>
public class InfluxDbListenerConfigV2 : InfluxDbListenerConfig
{
    /// <inheritdoc />
    public override string PluginName
    {
        get => nameof(InfluxDbListenerV2);
        set { }
    }

    /// <summary>
    /// Name of the database
    /// </summary>
    [DataMember]
    [Description("Organisation name")]
    public string Organisation { get; set; }

    /// <summary>
    /// API Token for access to influx. Can be used instead of username and password
    /// </summary>
    [DataMember]
    [Description("API Token for access to influx. Can be used instead of username and password")]
    [Password]
    public string Token { get; set; }

    /// <summary>
    /// Activate if the server has a trusted SSL certificate
    /// </summary>
    [DataMember]
    [DisplayName("Verify SSL Certificate"), Description("Activate if the server has a trusted SSL certificate")]
    public bool VerifySslCertificate { get; set; }

    /// <summary>
    /// Activate if the server has a trusted SSL certificate
    /// </summary>
    [DataMember]
    [DisplayName("Proxy Address"), Description("Adress of the proxy server to be used. Leave empty if non should be used.")]
    public string Proxy { get; set; }

    /// <summary>
    /// Activate if the server has a trusted SSL certificate
    /// </summary>
    [DataMember]
    [DisplayName("Bypass Proxy on Localhost"), Description("Activate if the server should bypass the proxy on localhost request.")]
    public bool BypassProxyOnLocalHost { get; set; }

    /// <summary>
    /// Number of retries after a failed attempt to write data
    /// </summary>
    [DataMember, DefaultValue(5)]
    [DisplayName("Retries"), Description("Maximum number of retries after a failed attempt to write data")]
    public short MaxNumberOfRetries { get; set; }
}
