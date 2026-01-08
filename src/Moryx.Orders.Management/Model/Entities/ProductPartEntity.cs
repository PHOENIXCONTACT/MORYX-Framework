// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.Orders.Management.Model;

public class ProductPartEntity : EntityBase
{
    public virtual string Name { get; set; }

    public virtual string Description { get; set; }

    public virtual string Number { get; set; }

    public virtual double Quantity { get; set; }

    public virtual string Unit { get; set; }

    public virtual int Classification { get; set; }

    public virtual int StagingIndicator { get; set; }

    public virtual long OperationId { get; set; }

    public virtual OperationEntity Operation { get; set; }
}