// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
namespace Moryx.Maintenance.IntervalTypes;

/// <summary>
/// Interval of type - DAY
/// </summary>
public class Days : IntervalBase
{
    const int HoursPerDay = 24;
    /// <inheritdoc/>
    public override void Update(int elapsed = 1)
    {
        var totalHours = (LastMaintenanceDate - UpdatedOn).TotalHours;
        if (totalHours < HoursPerDay)
        {
            return;
        }
        base.Update();
    }

    /// <summary>
    /// Returns the next maintenance date
    /// </summary>
    /// <returns>The date of the maintenance</returns>
    public DateTime NextDate()
    {
        return LastMaintenanceDate.AddDays(Value - Elapsed);
    }
}
