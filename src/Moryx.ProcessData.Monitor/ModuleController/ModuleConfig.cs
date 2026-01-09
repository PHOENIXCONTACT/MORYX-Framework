// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.ProcessData.Listener;
using Moryx.Serialization;

namespace Moryx.ProcessData.Monitor;

/// <summary>
/// Module configuration of the process data monitor <see cref="ModuleController"/>
/// </summary>
[DataContract]
public class ModuleConfig : ConfigBase
{
    /// <summary>
    /// List of listeners for process data events
    /// </summary>
    [DataMember, PluginConfigs(typeof(IProcessDataListener), true)]
    [Description("List of listeners for process data events")]
    public List<ProcessDataListenerConfig> Listeners { get; set; }
}