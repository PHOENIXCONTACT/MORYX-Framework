// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Shifts;

/// <summary>
/// Class representing a shift type.
/// </summary>
public class ShiftType : IPersistentObject
{
    /// <summary>
    /// The ID of the shift type.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The name of the shift type.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The start time of the shift type.
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// The end time of the shift type.
    /// </summary>
    public TimeOnly Endtime { get; set; }

    /// <summary>
    /// The period of the shift type.
    /// </summary>
    public byte Periode { get; set; }

    /// <summary>
    /// Creates a new instance with the given parameters
    /// </summary>
    public ShiftType(string name)
    {
        Name = name;
    }
}