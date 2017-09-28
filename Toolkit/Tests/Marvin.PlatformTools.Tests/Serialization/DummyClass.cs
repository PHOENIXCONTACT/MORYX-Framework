using System.Collections.Generic;

namespace Marvin.PlatformTools.Tests
{
    public class DummyClass
    {
        public int Number { get; set; }
 
        public string Name { get; set; }

        public SubClass SingleClass { get; set; }

        public SubClass[] SubArray { get; set; }

        public List<SubClass> SubList { get; set; } 

        public IEnumerable<SubClass> SubEnumerable { get; set; }

        public IDictionary<int, SubClass> SubDictionary { get; set; }
    }


    public class SubClass
    {
        public float Foo { get; set; }

        public DummyEnum Enum { get; set; }
    }

    public enum DummyEnum
    {
        Unset,
        ValueA,
        ValueB
    }
}