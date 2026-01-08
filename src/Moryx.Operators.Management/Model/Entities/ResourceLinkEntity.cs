// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Moryx.Model;

namespace Moryx.Operators.Management.Model;

public class ResourceLinkEntity : EntityBase
{
    public long ResourceId { get; set; }
}
