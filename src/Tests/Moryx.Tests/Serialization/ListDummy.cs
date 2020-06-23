// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Tests
{
    public class ListDummy
    {
        public byte Number { get; set; }

        public List<double> DoubleList { get; set; }

        public List<DummyEnum> EnumList { get; set; }
    }
}
