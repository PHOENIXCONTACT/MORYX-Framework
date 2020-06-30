// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using System.Collections.Generic;

namespace Moryx.Products.Model
{
    public class StepEntity : ModificationTrackedEntityBase
    {
        public virtual long StepId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Assembly { get; set; }

        public virtual string NameSpace { get; set; }

        public virtual string Classname { get; set; }

        public virtual string Parameters { get; set; }

        public virtual long WorkplanId { get; set; }

        public virtual long? SubWorkplanId { get; set; }

        public virtual WorkplanEntity Workplan { get; set; }

        public virtual WorkplanEntity SubWorkplan { get; set; }

        public virtual ICollection<ConnectorReference> Connectors { get; set; }

        public virtual ICollection<OutputDescriptionEntity> OutputDescriptions { get; set; }
    }
}
