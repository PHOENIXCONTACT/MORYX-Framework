// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.ProcessData.Configuration;

namespace Moryx.ProcessData.Adapter.NotificationPublisher;

/// <summary>
/// Module configuration of the adapter <see cref="ModuleController"/>
/// </summary>
[DataContract]
public class ModuleConfig : ConfigBase
{
    /// <summary>
    /// Creates a new instance of <see cref="ModuleConfig"/>
    /// </summary>
    public ModuleConfig()
    {
        NotificationBindings = new List<MeasurementBinding>
        {
            new() {Name = "source", Binding = "Notification.Source", ValueTarget = ValueTarget.Tag},
            new() {Name = "sender", Binding = "Notification.Sender", ValueTarget = ValueTarget.Tag},
            new() {Name = "acknowledger", Binding = "Notification.Acknowledger", ValueTarget = ValueTarget.Tag}

        };
    }

    /// <summary>
    /// Additional process measurement value bindings
    /// </summary>
    [DataMember]
    [Description("The data bound to this Measurand is gathered on any change of a notification within the system.")]
    public List<MeasurementBinding> NotificationBindings { get; set; }
}