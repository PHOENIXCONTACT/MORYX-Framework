using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marvin.Tests.Properties;

namespace Marvin.Tests
{
    [ClassDisplay(Name = nameof(strings.CrashTestDummy), ResourceType = typeof(strings))]
    public class DummyClass
    {
        [Display(Name = nameof(strings.Number), Description = nameof(strings.NumberDescription), ResourceType = typeof(strings))]
        public int Number { get; set; }

        public string Name { get; set; }

        public int ReadOnly => 10;

        [Display(Name = nameof(strings.SubClass), Description = nameof(strings.SubClassDescription), ResourceType = typeof(strings))]
        public SubClass SingleClass { get; set; }

        public SubClass[] SubArray { get; set; }

        public List<SubClass> SubList { get; set; }

        public IEnumerable<SubClass> SubEnumerable { get; set; }

        public IDictionary<int, SubClass> SubDictionary { get; set; }

        public DummyEnum[] EnumArray { get; set; }

        public List<DummyEnum> EnumList { get; set; }

        public IEnumerable<DummyEnum> EnumEnumerable { get; set; }

        public bool[] BoolArray { get; set; }

        public List<bool> BoolList { get; set; }

        public IEnumerable<DummyEnum> BoolEnumerable { get; set; }

        [Display(Name = nameof(strings.SubClass), Description = nameof(strings.SubClassDescription))]
        public SubClass SingleClassNonLocalized { get; set; }
    }

    [ClassDisplay(Name = nameof(strings.NoShownAsProperty), Description = nameof(strings.NoShownAsPropertyDescription), ResourceType = typeof(strings))]
    public class SubClass
    {
        public float Foo { get; set; }

        public DummyEnum Enum { get; set; }

        public DummyEnum[] EnumArray { get; set; }
    }

    public enum DummyEnum
    {
        Unset,
        ValueA,
        ValueB
    }
}