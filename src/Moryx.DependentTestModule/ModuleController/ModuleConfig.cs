// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.DependentTestModule
{
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        public int Foo { get; set; }
    }
}
