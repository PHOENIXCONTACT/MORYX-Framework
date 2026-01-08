// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Shifts.Endpoints.Models;

/// <summary>
/// Class representing the context for creating a shift type.
/// </summary>
[DataContract]
public class ShiftTypeCreationContextModel
{
    /// <summary>
    /// The name of the shift type.
    /// </summary>
    [DataMember]
    public string? Name { get; set; }

    /// <summary>
    /// The start time of the shift type.
    /// </summary>
    [DataMember]
    //[DataType(DataType.Time)]
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// The duration of the shift type.
    /// </summary>
    [DataMember]
    //[DataType(DataType.Time)]
    public TimeOnly Endtime { get; set; }

    /// <summary>
    /// The period of the shift type in days.
    /// </summary>
    [DataMember]
    public byte Periode { get; set; }
}