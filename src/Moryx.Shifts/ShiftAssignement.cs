// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Operators;

namespace Moryx.Shifts;

/// <summary>
/// Class representing a shift assignment.
/// </summary>
public class ShiftAssignement
{
    /// <summary>
    /// The ID of the shift assignment.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The resource of the shift assignment.
    /// </summary>
    public IResource Resource { get; set; }

    /// <summary>
    /// The operator of the shift assignment.
    /// </summary>
    public Operator Operator { get; set; }

    /// <summary>
    /// The shift of the shift assignment.
    /// </summary>
    public Shift Shift { get; set; }

    /// <summary>
    /// The note of the shift assignment.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// The priority of the shift assignment.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// The assigned days of the shift assignment.
    /// </summary>
    public AssignedDays AssignedDays { get; set; }

    /// <summary>
    /// Creates a new ShiftAssignement with the given parameters
    /// </summary>
    public ShiftAssignement(IResource resource, Operator @operator, Shift shift)
    {
        Resource = resource;
        Operator = @operator;
        Shift = shift;
    }
}