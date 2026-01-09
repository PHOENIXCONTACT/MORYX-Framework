// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Models;

[DataContract]
public class MachineGroupModel
{
    [DataMember]
    public virtual long Id { get; set; }

    [DataMember]
    public virtual CellLocationModel Location { get; set; }

    [DataMember]
    public virtual string IconName { get; set; }

    [DataMember]
    public virtual string Name { get; set; }
}