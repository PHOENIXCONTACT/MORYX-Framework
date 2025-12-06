// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Moryx.Runtime.Kernel.Tests.Configuration.ValueProvider
{
    public class TestConfig4
    {
        [DataMember]
        public List<int> Numbers { get; set; }

        [DataMember]
        public List<string> Strings { get; set; }

        [DataMember]
        public int[] ArrayNumbers { get; set; }

        [DataMember]
        public IEnumerable<int> EnumerableNumbers { get; set; }
    }
}
