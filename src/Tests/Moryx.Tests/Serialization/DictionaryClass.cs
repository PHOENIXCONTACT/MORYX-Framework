// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Tests
{
    class DictionaryClass
    {
        public string Name { get; set; }

        public IDictionary<string, int> SubDictionary { get; set; }

        public IDictionary<string, DummyEnum> EnumDictionary { get; set; }
    }
}

