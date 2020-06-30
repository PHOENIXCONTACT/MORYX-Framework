// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable once CheckNamespace
namespace Marvin.Resources.Model
{
    public class ResourceEntity : ModificationTrackedEntityBase
    {
        [Index]
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string ExtensionData { get; set; }

        public virtual string Type { get; set; }

        public virtual ICollection<ResourceRelation> Targets { get; set; }

        public virtual ICollection<ResourceRelation> Sources { get; set; }
    }
}
