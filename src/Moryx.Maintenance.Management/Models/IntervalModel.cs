// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Maintenance.IntervalTypes;
using Moryx.Serialization;

namespace Moryx.Maintenance.Management.Models;

public class IntervalModel
{
    [EntrySerialize]
    public IntervalBase Interval { get; set; }

    public IntervalType Type { get;  set; }
}

public enum IntervalType
{
    Day,
    Hour,
    Cycle
}
