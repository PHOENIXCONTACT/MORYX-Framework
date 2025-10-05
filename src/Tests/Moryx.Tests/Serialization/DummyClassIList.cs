// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Tests
{
    public class DummyClassIList
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public int ReadOnly => 10;

        public SubClass SingleClass { get; set; }

        public SubClass[] SubArray { get; set; }

        public List<SubClass> SubList { get; set; }

        public IList<int> SubIList { get; set; }

        public IEnumerable<SubClass> SubEnumerable { get; set; }

        public IDictionary<int, SubClass> SubDictionary { get; set; }

        public DummyEnum[] EnumArray { get; set; }

        public List<DummyEnum> EnumList { get; set; }

        public IEnumerable<DummyEnum> EnumEnumerable { get; set; }

        public bool[] BoolArray { get; set; }

        public List<bool> BoolList { get; set; }

        public IEnumerable<DummyEnum> BoolEnumerable { get; set; }

        public SubClass SingleClassNonLocalized { get; set; }
    }
}

