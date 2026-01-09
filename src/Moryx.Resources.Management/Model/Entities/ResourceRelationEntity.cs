// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.Resources.Management.Model;

public class ResourceRelationEntity : EntityBase
{
    public virtual int RelationType { get; set; }

    public virtual string SourceName { get; set; }

    public virtual long SourceId { get; set; }

    public virtual string TargetName { get; set; }

    public virtual long TargetId { get; set; }

    public virtual ResourceEntity Source { get; set; }

    public virtual ResourceEntity Target { get; set; }
}