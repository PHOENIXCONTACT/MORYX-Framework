// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moryx.ProcessData.Configuration;

/// <summary>
/// Binding configuration for a measurement
/// </summary>
[DataContract]
public class MeasurementBinding
{
    /// <summary>
    /// Process data value name
    /// </summary>
    [DataMember]
    [Description("Process data value name")]
    public string Name { get; set; }

    /// <summary>
    /// Binding string e.g. Process.Id
    /// </summary>
    [DataMember]
    [Description("Binding string e.g. Process.Id")]
    public string Binding { get; set; }

    /// <summary>
    /// Target of the binding value. Tags or fields.
    /// </summary>
    [DataMember]
    [Description("Target of the binding value. Tags or fields.")]
    public ValueTarget ValueTarget { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{Name}: {Binding}";
}