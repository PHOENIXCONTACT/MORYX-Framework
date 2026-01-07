// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
namespace Moryx.Maintenance;

/// <summary>
/// Interval of type - Hour
/// </summary>
public class Hours : IntervalBase
{
    public override void Update(int elapsed = 1)
    {
        var totalHours = (LastMaintenanceDate - UpdatedOn).TotalHours;
        if (totalHours < 1)
        {
            return;
        }
        base.Update();
    }
    /// <summary>
    /// Returns the next maintenance date-time
    /// </summary>
    /// <returns></returns>
    public DateTime NextDate()
    {
        return LastMaintenanceDate.AddHours(Value - Elapsed);
    }
}
