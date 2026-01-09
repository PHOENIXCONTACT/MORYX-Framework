// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Factory;

namespace Moryx.FactoryMonitor.Endpoints.Models;

/// <summary>
/// User data input to trace a route between 2 cells
/// </summary>
[DataContract]
public class TransportRouteModel
{
    [DataMember]
    public virtual long IdCellOfOrigin { get; set; }

    [DataMember]
    public virtual long IdCellOfDestination { get; set; }

    [DataMember]
    public virtual List<Position> Paths { get; set; }
}