// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.Products.Management.Model;

public class WorkplanStepEntity : EntityBase
{
    public virtual long StepId { get; set; }

    public virtual string Name { get; set; }

    public virtual string TypeName { get; set; }

    public virtual string Parameters { get; set; }

    public virtual long WorkplanId { get; set; }

    public virtual long? SubWorkplanId { get; set; }

    public virtual int PositionX { get; set; }

    public virtual int PositionY { get; set; }

    public virtual WorkplanEntity Workplan { get; set; }

    public virtual WorkplanEntity SubWorkplan { get; set; }

    public virtual ICollection<WorkplanConnectorReferenceEntity> Connectors { get; set; }

    public virtual ICollection<WorkplanOutputDescriptionEntity> OutputDescriptions { get; set; }

    public WorkplanStepEntity()
    {
        Connectors = new List<WorkplanConnectorReferenceEntity>();
        OutputDescriptions = new List<WorkplanOutputDescriptionEntity>();
    }
}
