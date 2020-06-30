// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using Moryx.Workflows;

namespace Moryx.Products.Model
{
    public class OutputDescriptionEntity : EntityBase
    {
        public virtual int Index { get; set; }

        public virtual int OutputType { get; set; }

        public virtual string Name { get; set; }

        public virtual long MappingValue { get; set; }

        public virtual long StepEntityId { get; set; }

        public virtual StepEntity Step { get; set; }
    }
}
