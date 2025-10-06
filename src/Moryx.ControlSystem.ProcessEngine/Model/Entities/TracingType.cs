// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    public class TracingType : EntityBase
    {
        public virtual string Assembly { get; set; }

        public virtual string NameSpace { get; set; }

        public virtual string Classname { get; set; }
    }
}

