// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Model;
using System.Collections.Generic;

namespace Marvin.Products.Model
{
    /// <summary>
    /// There are no comments for Marvin.Products.Model.ConnectorEntity in the schema.
    /// </summary>
    public class ConnectorEntity : EntityBase
    {
        public virtual long ConnectorId { get; set; }

        public virtual string Name { get; set; }

        public virtual int Classification { get; set; }

        public virtual long WorkplanId { get; set; }

        public virtual WorkplanEntity Workplan { get; set; }

        public virtual ICollection<ConnectorReference> Usages { get; set; }
    }
}
