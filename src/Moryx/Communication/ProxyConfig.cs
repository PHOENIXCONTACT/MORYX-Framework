// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Communication;

/// <summary>
/// Configuration for the web proxyies. Can be used as shared config for different modules.
/// </summary>
[DataContract]
public class ProxyConfig : ConfigBase
{
    /// <summary>
    /// Config entry to enable / disable the proxy.
    /// </summary>
    [DataMember]
    public bool EnableProxy { get; set; }

    /// <summary>
    /// Config entry to enable / disable the use of the default web proxy.
    /// </summary>
    [DataMember]
    public bool UseDefaultWebProxy { get; set; }

    /// <summary>
    /// Config entry for the proxy address.
    /// </summary>
    [DataMember]
    public string Address { get; set; }
}
