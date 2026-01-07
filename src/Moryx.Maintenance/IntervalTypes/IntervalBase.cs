// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Moryx.Serialization;

namespace Moryx.Maintenance;

/// <summary>
/// Type of the maintenance interval
/// </summary>
public abstract class IntervalBase
{
    /// <summary>
    /// Value configured for this interval type
    /// </summary>
    [EntrySerialize, Display(Name = "Configured interval", Description = "Value configured for this interval type")]
    [DefaultValue(35)]
    public int Value { get; set; }

    /// <summary>
    /// Total Elapsed interval. When it is equal <see cref="Value"/> then a maintenance should be performed
    /// </summary>
    [EntrySerialize, Display(Name = "Amount of time passed", Description = "When this value is equal to 'Configured time interval' a maintenance order will be sent to the resource.")]
    public int Elapsed { get; set; }

    /// <summary>
    /// Elapsed interval after a maintenance is due.
    /// this value increases when a maintenance is not performed
    /// </summary>
    [EntrySerialize(EntrySerializeMode.Never)]
    public int Overdue { get; set; }

    /// <summary>
    /// Display a warning before the maintenance is due
    /// </summary>
    [EntrySerialize,
      Display(
        Name = "Warning",
        Description = "Triggers a warning when the elapsed is greater or equal")]
    [DefaultValue(10)]
    public int Warning { get; set; }

    /// <summary>
    /// Date of the last maintenance
    /// </summary>
    [EntrySerialize(EntrySerializeMode.Never)]
    public DateTime LastMaintenanceDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the <see cref="Value"/> was updated
    /// </summary>
    [EntrySerialize(EntrySerializeMode.Never)]
    public DateTime UpdatedOn { get; set; }

    /// <summary>
    /// Resets the current <see cref="LastMaintenanceDate"/> to the current date and sets <see cref="Elapsed"/> and <see cref="Overdue"/> to 0
    /// </summary>
    public void Reset()
    {
        LastMaintenanceDate = DateTime.UtcNow;
        Elapsed = 0;
        Overdue = 0;
    }

    /// <summary>
    /// Update the interval <see cref="Value"/>
    /// </summary>
    public virtual void Update(int elapsed = 1)
    {
        Elapsed += elapsed;
        if (Elapsed > Value)
        {
            Overdue += 1;
        }
        UpdatedOn = DateTime.UtcNow;
    }

    public bool IsOverdue()
        => Overdue > 0;
    public bool IsDue()
        => Value == Elapsed;
}
