// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.Products.Model
{
    public class WorkplanReferenceEntity : EntityBase
    {
        public virtual int ReferenceType { get; set; }

        public virtual long SourceId { get; set; }

        public virtual long TargetId { get; set; }

        public virtual WorkplanEntity Target { get; set; }

        public virtual WorkplanEntity Source { get; set; }
    }
}
