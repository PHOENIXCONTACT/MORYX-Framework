// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations.Schema;
using Moryx.Model;

namespace Moryx.Products.Model
{
    public class WorkplanReference : EntityBase
    {
        public virtual int ReferenceType { get; set; }

        public virtual long SourceId { get; set; }

        public virtual long TargetId { get; set; }

        public virtual WorkplanEntity Target { get; set; }

        public virtual WorkplanEntity Source { get; set; }
    }
}
