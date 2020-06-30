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
