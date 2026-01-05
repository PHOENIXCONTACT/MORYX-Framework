// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
namespace Moryx.Maintenance.IntervalTypes;

/// <summary>
/// Interval of type - Cycle
/// </summary>
public class Cycles : IntervalBase
{

    /// <summary>
    /// Return the next cycle at which a maintenance should be done.
    /// </summary>
    /// <returns>Returns the number of cycle before the maintenance, or a negative value if the maintenance is overdue</returns>
    public int NextCycle()
    {
        return Value - Elapsed;
    }
}
