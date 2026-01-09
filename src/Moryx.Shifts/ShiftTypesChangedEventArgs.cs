// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Shifts;

/// <summary>
/// Class representing the event arguments for a shift type change.
/// </summary>
public class ShiftTypesChangedEventArgs : EventArgs
{
    /// <summary>
    /// The kind of change in the shift type.
    /// </summary>
    public ShiftTypeChange Change { get; set; }

    /// <summary>
    /// The shift type that was changed.
    /// </summary>
    public ShiftType Type { get; set; }

    /// <summary>
    /// Creates new event args with the given <paramref name="change"/> and <paramref name="assignement"/> parameters
    /// </summary>
    public ShiftTypesChangedEventArgs(ShiftTypeChange change, ShiftType type)
    {
        Change = change;
        Type = type;
    }
}