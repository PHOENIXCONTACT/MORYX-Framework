// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Tests.Extensions
{
    internal class ReferenceClass
    {
        public int Ignore { get; set; }

        public string NotRelevant { get; set; }

        public BaseClass BaseRef1 { get; set; }

        public ChildClass2 ChildRef { get; set; }

        public List<ChildClass1> Children1 { get; set; }

        public IEnumerable<ChildClass2> Children2 { get; set; }

        public GranChildClass1[] EmptyArray { get; set; }

        public GranChildClass2[] NullArray { get; set; }
    }
}
