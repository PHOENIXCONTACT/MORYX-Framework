// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using System.Collections.Generic;

namespace Moryx.Products.Model
{
    public class WorkplanEntity : ModificationTrackedEntityBase
    {
        public virtual string Name { get; set; }

        public virtual int Version { get; set; }

        public virtual int State { get; set; }

        public virtual int MaxElementId { get; set; }

        public virtual ICollection<ProductRecipeEntity> Recipes { get; set; }

        public virtual ICollection<WorkplanReferenceEntity> SourceReferences { get; set; }

        public virtual ICollection<WorkplanReferenceEntity> TargetReferences { get; set; }

        public virtual ICollection<WorkplanConnectorEntity> Connectors { get; set; }

        public virtual ICollection<WorkplanStepEntity> Steps { get; set; }

        public virtual ICollection<WorkplanStepEntity> Parents { get; set; }
    }
}
