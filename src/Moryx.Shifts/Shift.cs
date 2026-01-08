// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Shifts;

/// <summary>
/// Class representing a shift.
/// </summary>
public class Shift : IPersistentObject
{
    /// <summary>
    /// Creates a new shift of the provided <paramref name="type"/>
    /// </summary>
    public Shift(ShiftType type) => Type = type;

    /// <summary>
    /// The ID of the shift.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The type of the shift.
    /// </summary>
    public ShiftType Type { get; set; }

    /// <summary>
    /// The date of the shift.
    /// </summary>
    public DateOnly Date { get; set; }
}