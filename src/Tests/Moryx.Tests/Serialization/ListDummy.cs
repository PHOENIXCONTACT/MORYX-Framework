// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.Serialization;

namespace Moryx.Tests
{
    public class ListDummy
    {
        public byte Number { get; set; }

        [PrimitiveValues(1.0, 13.37, 42.0)]
        public List<double> DoubleList { get; set; }

        public List<DummyEnum> EnumList { get; set; }

        public List<SubClass> ReadOnly => new List<SubClass> { new SubClass { Foo = 4.2f } };
    }
}
