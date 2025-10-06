// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

// ReSharper disable once CheckNamespace
namespace Moryx.Resources.Model
{
    public class ResourceEntity : ModificationTrackedEntityBase
    {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string ExtensionData { get; set; }

        public virtual string Type { get; set; }

        public virtual ICollection<ResourceRelationEntity> Targets { get; set; }

        public virtual ICollection<ResourceRelationEntity> Sources { get; set; }
    }
}
