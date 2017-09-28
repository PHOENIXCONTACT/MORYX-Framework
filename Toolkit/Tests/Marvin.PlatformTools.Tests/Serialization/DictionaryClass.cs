using System.Collections.Generic;

namespace Marvin.PlatformTools.Tests
{
    class DictionaryClass
    {
        public string Name { get; set; }

        public IDictionary<string, int> SubDictionary { get; set; }

        public IDictionary<string, DummyEnum> EnumDictionary { get; set; }
    }
}
