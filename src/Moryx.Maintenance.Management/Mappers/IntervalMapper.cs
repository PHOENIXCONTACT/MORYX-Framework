// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Maintenance;
using Moryx.Maintenance.Management.Models;

namespace Moryx.Maintenance.Management.Mappers;
public static class IntervalMapper
{
    public static IntervalModel? ToModel(this IntervalBase? interval)
        => interval is null
        ? null
        : new IntervalModel
        {
            Interval = interval,
            Type = ToType(interval)
        };

    public static IntervalType ToType(IntervalBase? interval)
        => interval switch
        {
            Days _ => IntervalType.Day,
            Hours _ => IntervalType.Hour,
            _ => IntervalType.Cycle,
        };
}
