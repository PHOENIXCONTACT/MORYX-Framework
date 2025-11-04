// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.Products.Management.Model;

public class WorkplanOutputDescriptionEntity : EntityBase
{
    public virtual int Index { get; set; }

    public virtual int OutputType { get; set; }

    public virtual string Name { get; set; }

    public virtual long MappingValue { get; set; }

    public virtual long WorkplanStepId { get; set; }

    public virtual WorkplanStepEntity WorkplanStep { get; set; }
}