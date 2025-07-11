﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.ProcessData.Listener;

namespace Moryx.ProcessData.InfluxDbListener
{
    /// <summary>
    /// Configuration for the <see cref="InfluxDbListener"/>
    /// </summary>
    [DataContract]
    public class InfluxDbListenerConfig : ProcessDataListenerConfig
    {
        /// <inheritdoc />
        public override string PluginName
        {
            get => nameof(InfluxDbListener);
            set { }
        }

        /// <summary>
        /// Host of the api endpoint
        /// </summary>
        [DataMember, DefaultValue("localhost")]
        [Description("Host of the api endpoint")]
        public string Host { get; set; }

        /// <summary>
        /// Port for the api endpoint
        /// </summary>
        [DataMember, DefaultValue(8086)]
        [Description("Port for the api endpoint")]
        public int Port { get; set; }

        /// <summary>
        /// Activate if the server allows TLS encryption
        /// </summary>
        [DataMember, DefaultValue(false)]
        [Description("Activate if the server allows TLS encryption")]
        public bool UseTls { get; set; }

        /// <summary>
        /// Username for this connection
        /// </summary>
        [DataMember]
        [Description("Username for this connection")]
        public string Username { get; set; }

        /// <summary>
        /// Password for this connection
        /// </summary>
        [DataMember]
        [Password]
        [Description("Password for this connection")]
        public string Password { get; set; }

        /// <summary>
        /// Name of the database or bucket
        /// </summary>
        [DataMember]
        [Description("Name of the database or bucket")]
        public string DatabaseName { get; set; }

        /// <summary>
        /// Interval in milliseconds for the report
        /// </summary>
        [DataMember, DefaultValue(10000)]
        [Description("Interval in milliseconds for the report")]
        public int ReportIntervalMs { get; set; }
    }

    /// <summary>
    /// Extended config for influx 2
    /// </summary>
    public class InfluxDbListenerConfigV2 : InfluxDbListenerConfig
    {
        /// <inheritdoc />
        public override string PluginName
        {
            get =>  nameof(InfluxDbListenerV2);
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
        [DataMember, DefaultValue(false)]
        [DisplayName("Verify SSL Certificate"), Description("Activate if the server has a trusted SSL certificate")]
        public bool VerifySslCertificate { get; set; }

        /// <summary>
        /// Activate if the server has a trusted SSL certificate
        /// </summary>
        [DataMember]
        [DisplayName("Proxy Adress"), Description("Adress of the proxy server to be used. Leave empty if non should be used.")]
        public string Proxy { get; set; }

        /// <summary>
        /// Activate if the server has a trusted SSL certificate
        /// </summary>
        [DataMember, DefaultValue(false)]
        [DisplayName("Bypass Proxy on Localhost"), Description("Activate if the server should bypass the proxy on localhost request.")]
        public bool BypassProxyOnLocalHost { get; set; }

        /// <summary>
        /// Number of retries after a failed attempt to write data
        /// </summary>
        [DataMember, DefaultValue(5)]
        [DisplayName("Retries"),Description("Maximum number of retries after a failed attempt to write data")]
        public short MaxNumberOfRetries { get; set; }
    }
}

