// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Models;

/// <summary>
/// Model for the cell information for every ResourceUpdated event.
/// </summary>
[DataContract]
public class ResourceChangedModel : VisualizableItemModel
{
    [DataMember]
    public virtual string CellName { get; set; }

    // ToDo: Verify why this is duplicated and not using the location of the visual item
    [DataMember]
    public virtual string CellIconName { get => base.IconName; set => base.IconName = value; }

    [DataMember]
    public virtual string CellImageURL { get; set; }

    // ToDo: Verify why this is duplicated and not using the location of the visual item
    [DataMember]
    public long Id { get => base.Id; set => base.Id = value; }

    // ToDo: Verify why this is duplicated and not using the location of the visual item
    [DataMember]
    public CellLocationModel CellLocation { get => base.Location; set => base.Location = value; }

    [DataMember]
    public Dictionary<string, CellPropertySettings> CellPropertySettings { get; set; }

    [DataMember]
    public long FactoryId { get; internal set; }
}
